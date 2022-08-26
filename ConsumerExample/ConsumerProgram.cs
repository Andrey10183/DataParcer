using LAB.DataScanner.Components.Interfaces;
using LAB.DataScanner.Components.Services.MessageBroker;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ConsumerExample
{
    class ConsumerProgram
    {
        static string queue = "exampleTestQueue2";
        static string exchange = "exampleExchangeTopicDurable";
        static string exchange2 = "exampleExchange2";
        static string routingKey1 = "exampleRoutingKey1";
        static string routingKey2 = "exampleRoutingKey2";
        private static IRmqConsumer consumer;
        private static RmqConsumerBuilder consumerBuilder;

        private static void OnRecieve(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine(" [x] Received '{0}':'{1}'",
                              routingKey, message);
            //consumer.Ack(ea);
        }

        static void Main(string[] args)
        {
            consumerBuilder = new RmqConsumerBuilder();
            consumerBuilder.UsingDefaultConnectionSetting();
            consumerBuilder.UsingQueue(queue);
            consumerBuilder.AddBinding("ConverterExchange", "#");
            consumer = consumerBuilder.Build();
            consumer.StartListening((s,ea) => OnRecieve(s,ea));

            Console.WriteLine("Listening. Press [enter] to exit...");
            Console.ReadLine();
            
            consumer.StopListening();
            consumer.Dispose();
        }
    }
}
