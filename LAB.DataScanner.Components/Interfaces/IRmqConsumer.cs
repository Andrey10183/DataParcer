using RabbitMQ.Client.Events;
using System;

namespace LAB.DataScanner.Components.Interfaces
{
    public interface IRmqConsumer : IDisposable
    {
        void Ack(BasicDeliverEventArgs args);
        public void StartListening(EventHandler<BasicDeliverEventArgs> onReceiveHandler);
        public void StopListening();
        public void SetQueue(string queueName);
    }
}
