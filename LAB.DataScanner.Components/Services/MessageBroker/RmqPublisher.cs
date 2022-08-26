using LAB.DataScanner.Components.Interfaces;
using RabbitMQ.Client;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LAB.DataScanner.Components.Tests")]

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    //This component should be able to
    //publish any message to RabbitMQ.

    public class RmqPublisher : IRmqPublisher
    {
        private readonly IModel _ampqChannel;
        private string _exchange;
        private string _routingKey;

        internal RmqPublisher(IModel ampqChannel, string exchange, string routingKey)
        {
            _ampqChannel = ampqChannel;
            _exchange = exchange;
            _routingKey = routingKey;           
        }
        
        public void Dispose()
        {
            _ampqChannel.Close();
            _ampqChannel.Dispose();
        }

        public void Publish(byte[] message)
        {
            _ampqChannel.BasicPublish(
                exchange: _exchange,
                routingKey: _routingKey,
                basicProperties: null,
                body: message);
        }

        public void Publish(byte[] message, string routingKey)
        {
            _ampqChannel.BasicPublish(
                exchange: _exchange,
                routingKey: routingKey,
                basicProperties: null,
                body: message);
        }

        public void Publish(byte[] message, string exchange, string routingKey)
        {
            _ampqChannel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: null,
                body: message);
        }

        public void Publish(byte[] outputData, string exchangeName, string[] routingKeys)
        {
            foreach (var routingKey in routingKeys)
                _ampqChannel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: outputData);
        }
    }
}
