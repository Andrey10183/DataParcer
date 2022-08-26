using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using LAB.DataScanner.Components.Services.Downloader;
using LAB.DataScanner.Components.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace LAB.DataScanner.Components.Tests.Unit.Services.WebPadeDownloader
{
    class WebPageDownloaderEngineTests
    {
        [Test]
        public void Constructor_NullConfiguration_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new WebPageDownloaderEngine(
                null,
                new Mock<IDataRetriever>().Object,
                new Mock<IRmqPublisher>().Object,
                new Mock<IRmqConsumer>().Object));

        [Test]
        public void Constructor_NullDataRetriever_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new WebPageDownloaderEngine(
                new Mock<IConfiguration>().Object,
                null,
                new Mock<IRmqPublisher>().Object,
                new Mock<IRmqConsumer>().Object));

        [Test]
        public void Constructor_NullPublisher_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new WebPageDownloaderEngine(
                new Mock<IConfiguration>().Object,
                new Mock<IDataRetriever>().Object,
                null,
                new Mock<IRmqConsumer>().Object));

        [Test]
        public void Constructor_NullConsumer_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new WebPageDownloaderEngine(
                new Mock<IConfiguration>().Object,
                new Mock<IDataRetriever>().Object,
                new Mock<IRmqPublisher>().Object,
                null));

        [Test, Sequential]
        public void Constructor_InvalidConfiguration_ThrowsArgumentNullException(
            [Values(null, "queue", "queue")] string queue,
            [Values("exchange", null, "exchange")] string exchange,
            [Values("routing", "routing", null)] string routing)
        {
            var config = new Dictionary<string, string>
            {
                { "Binding:ReceiverQueue", queue},
                { "Binding:SenderExchange", exchange},
                { "Binding:SenderRoutingKeys", routing}
            };

            var fakeConfigurationSection = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            Assert.Throws<ArgumentNullException>(() => new WebPageDownloaderEngine(
                fakeConfigurationSection, 
                new Mock<IDataRetriever>().Object,
                new Mock<IRmqPublisher>().Object,
                new Mock<IRmqConsumer>().Object));
        }

        [Test]
        public void ShouldPublishToExchangePageAsIsOnceSucessfullDownload()
        {
            var config = new Dictionary<string, string>
            {
                 
                { "Binding:ReceiverQueue", "TestQueue"},
                { "Binding:ReceiverExchange", "ReceiverExchange"},
                { "Binding:ReceiverRoutingKeys:0", "recieveRK1"},
                { "Binding:SenderExchange", "TestExchange"},
                { "Binding:SenderRoutingKeys:0", "TestRouting"}
            };

            var fakeConfigurationSection = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            var publisher = new Mock<IRmqPublisher>();
            var retriver = new Mock<IDataRetriever>();

            var downloader = new WebPageDownloaderEngine(
                fakeConfigurationSection,
                retriver.Object,
                publisher.Object,
                new Mock<IRmqConsumer>().Object);

            string[] urls = new string[] {
                "https://yandex.ru/1/",
                "https://yandex.ru/2/",
                "https://yandex.ru/3/",
                "https://yandex.ru/4/"
            };

            retriver
                .Setup(x => x.RetrieveBytesAsync(It.IsAny<string>()))
                .ReturnsAsync(new byte[] { 1, 2, 3 });

            foreach (var url in urls)
                downloader._urlList.Enqueue(url);

            downloader.Start();
            Thread.Sleep(100);
            retriver
                .Verify(x => x.RetrieveBytesAsync(It.IsAny<string>()), Times.Exactly(4));

            publisher
                .Verify(x => x.Publish(new byte[] { 1, 2, 3}, "TestExchange", new string[] { "TestRouting" }), Times.Exactly(4));
        }
    }
}
