using LAB.DataScanner.Components.Models;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public abstract class RmqBuilder<T>
        where T : class
    {
        protected string UserName { get; set; }
        protected string Password { get; set; }
        protected string HostName { get; set; }
        protected int Port { get; set; }
        protected string VirtualHost { get; set; }
        protected IConnectionFactory ConnFactory { get; set; }
        protected IConnection Connection { get; set; }
        protected IModel Channel { get; set; }

        protected RmqBuilder() => SetDefaultSettings();

        public RmqBuilder<T> UsingDefaultConnectionSetting()
        {
            SetDefaultSettings();
            return this;
        }

        public RmqBuilder<T> UsingConfigConnectionSettings(IConfiguration configuration)
        {
            var rmqConfiguration = configuration
                .GetSection(nameof(RmqConfiguration.RmqConfig))
                .Get<RmqConfiguration>();

            UserName = rmqConfiguration.UserName;
            Password = rmqConfiguration.Password;
            HostName = rmqConfiguration.HostName;
            VirtualHost = rmqConfiguration.VirtualHost;
            Port = rmqConfiguration.Port;

            return this;
        }

        public RmqBuilder<T> UsingCustomHost(string hostName)
        {
            HostName = hostName;
            return this;
        }

        public RmqBuilder<T> UsingCustomCredentials(string userName, string userPassword)
        {
            UserName = userName;
            Password = userPassword;
            return this;
        }

        public abstract T Build();

        private void SetDefaultSettings()
        {
            UserName = "guest";
            Password = "guest";
            HostName = "localhost";
            Port = 5672;
            VirtualHost = "/";
        }

        protected void PrepareConnection()
        {
            ConnFactory = new ConnectionFactory
            {
                UserName = UserName,
                Password = Password,
                HostName = HostName,
                Port = Port,
                VirtualHost = VirtualHost
            };

            try
            {
                Connection = ConnFactory.CreateConnection();
                Channel = Connection.CreateModel();
            }
            catch (BrokerUnreachableException)
            {
                throw new NullReferenceException("Connection to rabbitmq server failed.");
            }
        }
    }
}
