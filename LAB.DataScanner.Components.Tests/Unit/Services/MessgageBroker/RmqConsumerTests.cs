using LAB.DataScanner.Components.Services.MessageBroker;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace LAB.DataScanner.Components.Tests
{
    public class RmqConsumerTests
    {
        private Mock<IModel> mockedChannel;
        private string _queueName = "queue";
        private RmqConsumer rmqConsumer;
        private event EventHandler<BasicDeliverEventArgs> handler;

        protected virtual void OnRecieve(object sender, BasicDeliverEventArgs ea)
        {
            handler?.Invoke(sender, ea);
        }

        [SetUp]
        public void Setup()
        {
            mockedChannel = new Mock<IModel>();
            rmqConsumer = new RmqConsumer(mockedChannel.Object, _queueName);
        }

        [Test]
        public void ShouldCall_AckMessage_OnceTheyArrivedAndHandled()
        {
            var ea = new BasicDeliverEventArgs() { DeliveryTag = It.IsAny<ulong>() };
            
            rmqConsumer.Ack(ea);

            mockedChannel.Verify(mc => mc.BasicAck(It.IsAny<ulong>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public void ShouldCall_BasicConsume_OnceStartListening()
        {
            rmqConsumer.StartListening(OnRecieve);
            mockedChannel
                .Verify(m => m.BasicConsume(
                    It.IsAny<string>(), 
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<IBasicConsumer>()), Times.Once);
        }

        [Test]
        public void ShouldCall_BasicCancel_OnceStopListening()
        {
            rmqConsumer.StopListening();
            mockedChannel.Verify(mc => mc.BasicCancel(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SetQueue_NullOrEmptyQueue_throwArgumentException() =>
            Assert.Throws<ArgumentNullException>(() => rmqConsumer.SetQueue(null));
    }
}
