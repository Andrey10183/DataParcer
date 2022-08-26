using LAB.DataScanner.Components.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Runtime.CompilerServices;

//[assembly: InternalsVisibleTo("LAB.DataScanner.Components.Tests")]

//This components should listen for messages in RabbitMQ and react once message
//arrives using onReceiveHandler.If we call StopListening - we should unsubscribe
//from RabbitMQ and dispose connection with it.

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqConsumer : IRmqConsumer
    {
        private string _queueName;
        private readonly IModel _ampqChannel;
        private readonly EventingBasicConsumer _consumer;
        private string _consumerTag;

        internal RmqConsumer(IModel amqpChannel, string queueName)
        {
            _ampqChannel = amqpChannel;
            
            _queueName = queueName;
            
            _consumer = new EventingBasicConsumer(_ampqChannel);
        }
        
        public void Ack(BasicDeliverEventArgs args)
        {
            _ampqChannel.BasicAck(args.DeliveryTag, false);
        }

        public void Dispose()
        {
            _ampqChannel.Close();
            _ampqChannel.Dispose();
        }

        public void SetQueue(string queueName)
        {
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        public void StartListening(EventHandler<BasicDeliverEventArgs> onReceiveHandler)
        {
            _consumer.Received += onReceiveHandler;
            _consumerTag = _ampqChannel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: _consumer);
        }

        public void StopListening()
        {
            _ampqChannel.BasicCancel(_consumerTag);
        }
    }
}
