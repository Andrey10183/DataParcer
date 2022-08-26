using LAB.DataScanner.Components.Models;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.Persisters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;

namespace LAB.DataScanner.SimpleTableDBPersister
{
    class Program
    {
        static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //         .WriteTo.Console()
            //         .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            //         {
            //             AutoRegisterTemplate = true,
            //         })
            //         .CreateLogger();
            
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var loggerFactory = (ILoggerFactory)new LoggerFactory();
            loggerFactory.AddSerilog(Log.Logger);
            var logger = loggerFactory.CreateLogger<SimpleDBPersisterEngine>();

            var options = configuration
                .GetSection(nameof(BindingConfiguration.Binding))
                .Get<BindingConfiguration>();

            var contextOptions = new DbContextOptionsBuilder<PersisterDBContext>()
                .UseSqlServer(configuration.GetValue<string>("Application:SqlConnectionString"))
                .Options;

            var context = new PersisterDBContext(contextOptions);

            var rmqConsumer = new RmqConsumerBuilder()
                .UsingQueue(options.ReceiverQueue)
                .AddBindings(options.ReceiverExchange, options.ReceiverRoutingKeys)
                .UsingConfigConnectionSettings(configuration)
                .Build();

            var persister = new SimpleDBPersisterEngine(
                context,
                configuration,
                rmqConsumer,
                logger);
            
            persister.Start();

            Console.ReadLine();
        }
    }
}
