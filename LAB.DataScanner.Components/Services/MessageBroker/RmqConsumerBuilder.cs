using LAB.DataScanner.Components.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqConsumerBuilder : RmqBuilder<IRmqConsumer>
    {
        private string _queueName ="";        
        private bool _queueAutoCreation = false;
        private Dictionary<string, List<string>> _bindings = new Dictionary<string, List<string>>();

        public RmqConsumerBuilder UsingQueue(string queueName)
        {
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            
            return this;
        }

        public RmqConsumerBuilder UsingConfigQueueName(IConfigurationSection configurationSection)
        {
            if (configurationSection is null)
                throw new ArgumentNullException(nameof(configurationSection));

            _queueName = configurationSection["QueueName"];

            return this;
        }

        public RmqConsumerBuilder AddBinding(string exchange, string routingKey)
        {
            if (exchange is null)
                throw new ArgumentNullException(nameof(exchange));

            if (routingKey is null)
                throw new ArgumentNullException(nameof(routingKey));

            if (!_bindings.ContainsKey(exchange))
                _bindings[exchange] = new List<string>() { routingKey };
            else
                _bindings[exchange].Add(routingKey);
            
            return this;
        }

        public RmqConsumerBuilder AddBindings(string exchange, string[] routingKeys)
        {
            if (exchange is null)
                throw new ArgumentNullException(nameof(exchange));

            if (routingKeys is null)
                throw new ArgumentNullException(nameof(routingKeys));

            foreach (var key in routingKeys)
            {
                if (!_bindings.ContainsKey(exchange))
                    _bindings[exchange] = new List<string>() { key };
                else
                    _bindings[exchange].Add(key);
            }
            
            return this;
        }

        public RmqConsumerBuilder WithQueueAutoCreation()
        {
            _queueAutoCreation = true;
            return this;
        }

        public override IRmqConsumer Build()
        {
            PrepareConnection();

            if (_queueAutoCreation)
                _queueName = Channel.QueueDeclare().QueueName;            
            else
                Channel.QueueDeclare(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    arguments: null);

            foreach (var binding in _bindings)
            {
                Channel.ExchangeDeclare(
                    exchange: binding.Key, 
                    type: "topic",
                    durable: true);
                
                foreach (var routingKey in binding.Value)
                    Channel.QueueBind(
                        queue: _queueName,
                        exchange: binding.Key,
                        routingKey: routingKey);
            }

            return new RmqConsumer(Channel, _queueName);
        }
    }
}
