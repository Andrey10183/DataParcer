using LAB.DataScanner.Components.Interfaces;
using LAB.DataScanner.Components.Services.Generators;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace LAB.DataScanner.Components.Tests.Unit.Services.Generators
{
    public class UrlsGeneratorEngineTests
    {
        private Mock<IRmqPublisher> rmqPublisherServiceMock;

        [Test]
        public void ShouldGenerateAndPublishUrlsBasedOnConfiguration()
        {
            var configDic = new Dictionary<string, string>
            {
                { "Application:UrlTemplate", "http://testSite/{0}/{1}/{2}" },
                { "Application:Sequences:0", "0..1" },
                { "Application:Sequences:1", "2..3" },
                { "Application:Sequences:2", "4..5" },
                { "Binding:SenderExchange", "TargetExchange"},
                { "Binding:SenderRoutingKeys:0", "A" },
                { "Binding:SenderRoutingKeys:1", "B" },
            };

            var sut = SutPreparation(configDic);
            
            var messagesToPublish = new string[]
                {
                    "http://testSite/0/2/4",
                    "http://testSite/0/2/5",
                    "http://testSite/0/3/4",
                    "http://testSite/0/3/5",
                    "http://testSite/1/2/4",
                    "http://testSite/1/2/5",
                    "http://testSite/1/3/4",
                    "http://testSite/1/3/5",
                };
            
            //Act
            sut.Start();

            //Assert
            foreach (var message in messagesToPublish)
            {
                var body = Encoding.UTF8.GetBytes(message);
                rmqPublisherServiceMock
                    .Verify(x => x.Publish(body, "TargetExchange", new string[] { "A", "B" }), Times.Once);
            }

            rmqPublisherServiceMock
                    .Verify(x => x.Publish(It.IsAny<byte []>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Exactly(8));
        }

        [Test]
        public void NoExchangeInfo_ThrowArgumentException()
        {
            //Arrange
            var configDic = new Dictionary<string, string>
            {
                { "Application:UrlTemplate", "http://testSite/{0}" },
                { "Application:Sequences:0", "0..1" },
                { "Binding:SenderExchange", ""},
                { "Binding:SenderRoutingKeys:0", "A"}
            };

            var sut = SutPreparation(configDic);
            Assert.Throws<ArgumentException>(() => sut.Start());
        }

        [Test]
        public void NoRoutingInfo_ThrowArgumentException()
        {
            //Arrange
            var configDic = new Dictionary<string, string>
            {
                { "Application:UrlTemplate", "http://testSite/{0}" },
                { "Application:Sequences:0", "0..1" },
                { "Binding:SenderExchange", "TargetExchange"},
                { "Binding:SenderRoutingKeys", null}
            };

            var sut = SutPreparation(configDic);
            Assert.Throws<ArgumentException>(() => sut.Start());
        }

        [Test]
        public void MismatchTemplateAndSequences_ThrowArgumentException()
        {
            var configDic = new Dictionary<string, string>
            {
                { "Application:UrlTemplate", "http://testSite/{0}/{1}/{2}" },
                { "Application:Sequences:0", "0..1" },
                { "Application:Sequences:1", "2..3" },
                { "Binding:SenderExchange", "TargetExchange"},
                { "Binding:SenderRoutingKeys:0", "A"}
            };

            var sut = SutPreparation(configDic);
            Assert.Throws<ArgumentException>(() => sut.Start());
        }

        [Test]
        public void NullSequences_ThrowArgumentException()
        {
            var configDic = new Dictionary<string, string>
            {
                { "Application:UrlTemplate", "http://testSite/{0}/{1}/{2}" },
                { "Application:Sequences", "" },
                { "Binding:SenderExchange", "TargetExchange"},
                { "Binding:SenderRoutingKeys:0", "A"}
            };

            var sut = SutPreparation(configDic);
            Assert.Throws<ArgumentException>(() => sut.Start());
        }

        [Test]
        public void WrongSequences_ThrowArgumentException()
        {
            var configDic = new Dictionary<string, string>
            {
                { "Application:UrlTemplate", "http://testSite/{0}" },
                { "Application:Sequences:0", "abc" },
                { "Binding:SenderExchange", "TargetExchange"},
                { "Binding:SenderRoutingKeys:0", "A"}
            };

            var sut = SutPreparation(configDic);
            Assert.Throws<ArgumentException>(() => sut.Start());
        }

        [Test]
        public void Constructor_NullRmqPublisher_ThrowsArgumentNullException()
        {
            var fakeConfigurationSection = new ConfigurationBuilder()
               .AddInMemoryCollection(new Dictionary<string, string>())
               .Build();

            Assert.Throws<ArgumentNullException>(() => new UrlsGeneratorEngine(
                null,
                fakeConfigurationSection));
        }

        [Test]
        public void Constructor_NullConfigParam_ThrowsArgumentNullException()
        {
            rmqPublisherServiceMock = new Mock<IRmqPublisher>();

            Assert.Throws<ArgumentNullException>(() => new UrlsGeneratorEngine(
                rmqPublisherServiceMock.Object,
                null));
        }

        private UrlsGeneratorEngine SutPreparation(Dictionary<string, string> dict)
        {
            rmqPublisherServiceMock = new Mock<IRmqPublisher>();

            var fakeConfigurationSection = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();

            return new UrlsGeneratorEngine(
                rmqPublisherServiceMock.Object,
                fakeConfigurationSection);
        }
    }
}
