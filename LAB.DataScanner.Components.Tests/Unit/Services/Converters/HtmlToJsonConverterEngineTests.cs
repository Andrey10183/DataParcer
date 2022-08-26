using LAB.DataScanner.Components.Interfaces;
using LAB.DataScanner.Components.Services.Converters;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LAB.DataScanner.Components.Tests.Unit.Services.Converters
{
    public class HtmlToJsonConverterEngineTests
    {
        const string html = @"
            <html>
                <head></head>
                <body>
                    <product>
                        <h1 id='title'>Prod1</h1>
                        <h2 id='prop'>Prop1</h2>
                        <h3 id='style'>Style1</h2>
                    </product>
                    <product>
                        <h1 id='title'>Prod2</h1>
                        <h2 id='prop'>Prop2</h2>
                    </product>
                </body>";

        [Test]
        public void Constructor_NullConfiguration_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new HtmlToJsonConverterEngine(
                null,
                new Mock<IRmqPublisher>().Object,
                new Mock<IRmqConsumer>().Object));

        [Test]
        public void Constructor_NullPublisher_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new HtmlToJsonConverterEngine(
                new Mock<IConfiguration>().Object,
                null,
                new Mock<IRmqConsumer>().Object));

        [Test]
        public void Constructor_NullConsumer_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new HtmlToJsonConverterEngine(
                new Mock<IConfiguration>().Object,
                new Mock<IRmqPublisher>().Object,
                null));

        [Test]
        public void Constructor_UnknownStrategy_ThrowsArgumentNullException()
        {
            var dict = new Dictionary<string, string>() {
                { "Application:HtmlFragmentStrategy", "UnknownStrategy" },
                { "Application:HtmlFragmentExpression", "Expression" },
            };

            var fakeConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();

            Assert.Throws<ArgumentNullException>(() => new HtmlToJsonConverterEngine(
                fakeConfig,
                new Mock<IRmqPublisher>().Object,
                new Mock<IRmqConsumer>().Object));
        }

        [Test]
        public void ShouldPublishToExchangeJsonAsIsOnceSucessfullConverted()
        {
            var config = new Dictionary<string, string>
            {
                { "Application:HtmlFragmentStrategy","LAB.DataScanner.Components.Services.Converters.ConvertStrategies.CustomHtmlToJsonConverter"},
                { "Application:HtmlFragmentExpressions:DataItem1","//h1"},
                { "Binding:ReceiverQueue", "TestQueue"},
                { "Binding:ReceiverExchange", "ReceiverExchange"},
                { "Binding:ReceiverRoutingKeys:0", "recieveRK1"},
                { "Binding:SenderExchange", "TestExchange"},
                { "Binding:SenderRoutingKeys:0", "TestRouting"}
            };

            var fakeConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            var publisher = new Mock<IRmqPublisher>();
            var consumer = new Mock<IRmqConsumer>();
            var sut = new HtmlToJsonConverterEngine(
                fakeConfig,
                publisher.Object,
                consumer.Object);

            sut._htmlPagesList.Enqueue(html);

            sut.Start();
            Thread.Sleep(100);

            var expectedBytes = Encoding.UTF8.GetBytes("[{\"DataItem1\":\"Prod1\"},{\"DataItem1\":\"Prod2\"}]");
            
            publisher
                .Verify(x => x.Publish(expectedBytes, "TestExchange", new string[] { "TestRouting" }), Times.Once);
        }
    }
}
