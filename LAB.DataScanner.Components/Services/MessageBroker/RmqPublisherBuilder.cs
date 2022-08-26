using LAB.DataScanner.Components.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqPublisherBuilder : RmqBuilder<IRmqPublisher>
    {
        private string _exchange = "";
        private string _routingKey = "";
        private bool _exchangeAutoCreate = false;
        
        public RmqPublisherBuilder UsingExchange(string exchange)
        {
            _exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            
            return this;
        }

        public RmqPublisherBuilder UsingRoutingKey(string routingKey)
        {
            _routingKey = routingKey ?? throw new ArgumentNullException(nameof(routingKey));
            
            return this;
        }

        public RmqPublisherBuilder UsingExchangeAndRoutingKey(string exchange, string routingKey)
        {
            _exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            _routingKey = routingKey ?? throw new ArgumentNullException(nameof(routingKey));

            return this;
        }

        public RmqPublisherBuilder UsingConfigExchangeAndRoutingKey(IConfigurationSection configurationSection)
        {
            if (configurationSection is null)
                throw new ArgumentNullException(nameof(configurationSection));

            _exchange = configurationSection["Exchange"];
            _routingKey = configurationSection["RoutingKey"];

            return this;
        }

        public RmqPublisherBuilder WithExchangeAutoCreation()
        {
            _exchangeAutoCreate = true;
            throw new NotImplementedException();
        }
        
        public override IRmqPublisher Build()
        {
            PrepareConnection();

            //Channel.ExchangeDeclare(
            //    exchange: _exchange,
            //    type: "topic",
            //    durable: true);

            return new RmqPublisher(Channel, _exchange, _routingKey);
        }        
    }
}
