using LAB.DataScanner.Components.Services.MessageBroker;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;

namespace LAB.DataScanner.Components.Tests
{
    public class RmqPublisherTests
    {
        private Mock<IModel> _mockedChannel;
        private RmqPublisher _rmqPublisher;
        private string _defaultExchange = "exchange";
        private string _defaultRoutingKey = "routingKey";
        private byte[] message = new byte[3] { 1, 2, 3};

        [SetUp]
        public void Setup()
        {
            _mockedChannel = new Mock<IModel>();
            _rmqPublisher = new RmqPublisher(_mockedChannel.Object, _defaultExchange, _defaultRoutingKey);
        }

        [Test]
        public void ShouldPublishMessageToDefaultExchange()
        {
            _rmqPublisher.Publish(message);
            _mockedChannel
                .Verify(mc => mc.BasicPublish(_defaultExchange, _defaultRoutingKey, It.IsAny<bool>(), It.IsAny<IBasicProperties>(), message), Times.Once);
        }

        [Test]
        public void ShouldPublishMessageWithRoutingKey()
        {
            var specificKety = "specificKety";
            _rmqPublisher.Publish(message, specificKety);
            _mockedChannel
                .Verify(mc => mc.BasicPublish(_defaultExchange, specificKety, It.IsAny<bool>(), It.IsAny<IBasicProperties>(), message), Times.Once);
        }

        [Test]
        public void ShouldPublishMessageToCertainExchangeAndRoutingKey()
        {
            var specificKety = "specificKety";
            var specificExchange = "specificExchange";
            _rmqPublisher.Publish(message, specificExchange, specificKety);
            _mockedChannel
                .Verify(mc => mc.BasicPublish(specificExchange, specificKety, It.IsAny<bool>(), It.IsAny<IBasicProperties>(), message), Times.Once);
        }
    }
}

