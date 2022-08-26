using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using LAB.DataScanner.Components.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading;
using LAB.DataScanner.Components.Services.Persisters;
using LAB.DataScanner.Components.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LAB.DataScanner.Components.Tests.Unit.Services.Persisters
{
    class SimpleDBPersisterEngineTests
    {
        const string json = "[{\"DataItem1\":\"item1\",\"DataItem2\":\"item2\",\"DataItem3\":\"item3\"}]";
        
        [Test]
        public void Constructor_NullContext_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new SimpleDBPersisterEngine(
                null,
                new Mock<IConfiguration>().Object,
                new Mock<IRmqConsumer>().Object));

        [Test]
        public void Constructor_NullConfig_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new SimpleDBPersisterEngine(
                new Mock<IPersisterDBContext>().Object,
                null,
                new Mock<IRmqConsumer>().Object));

        [Test]
        public void Constructor_NullConsumer_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new SimpleDBPersisterEngine(
                new Mock<IPersisterDBContext>().Object,
                new Mock<IConfiguration>().Object,
                null));

        [Test]
        public void ShouldSaveDataToContextOnceRecievedJson()
        {
            var config = new Dictionary<string, string>
            {

                { "Binding:ReceiverQueue", "TestQueue"},
                { "Binding:ReceiverExchange", "ReceiverExchange"},
                { "Binding:ReceiverRoutingKeys:0", "recieveRK1"},
                { "Binding:SenderExchange", "TestExchange"},
                { "Binding:SenderRoutingKeys:0", "TestRouting"}
            };

            var fakeConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            var consumer = new Mock<IRmqConsumer>();

            var options = new DbContextOptionsBuilder<PersisterDBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            using (var context = new PersisterDBContext(options))
            {
                var sut = new SimpleDBPersisterEngine(
                    context,
                    fakeConfig,
                    consumer.Object);

                sut._jsonList.Enqueue(json);

                sut.Start();
                Thread.Sleep(100);

                var item = new PersisterModel()
                {
                    DataItem1 = "item1",
                    DataItem2 = "item2",
                    DataItem3 = "item3",
                };

                var result = context.Data.ToArray();
                var expected = new PersisterModel[] { item };
                Assert.AreEqual(item.DataItem1, result[0].DataItem1);
                Assert.AreEqual(item.DataItem2, result[0].DataItem2);
                Assert.AreEqual(item.DataItem3, result[0].DataItem3);
            }                
        }
    }
}
