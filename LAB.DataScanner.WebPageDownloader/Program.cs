using LAB.DataScanner.Components.Models;
using LAB.DataScanner.Components.Services.Downloader;
using LAB.DataScanner.Components.Services.MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LAB.DataScanner.WebPageDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                     .WriteTo.Console()
                     .CreateLogger();

            var loggerFactory = (ILoggerFactory)new LoggerFactory();
            loggerFactory.AddSerilog(Log.Logger);
            var logger = loggerFactory.CreateLogger<WebPageDownloaderEngine>();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var options = configuration
                .GetSection(nameof(BindingConfiguration.Binding))
                .Get<BindingConfiguration>();

            var rmqPublisher = new RmqPublisherBuilder()
                .UsingDefaultConnectionSetting()
                .Build();

            var rmqConsumer = new RmqConsumerBuilder()
                .UsingQueue(options.ReceiverQueue)
                .AddBindings(options.ReceiverExchange, options.ReceiverRoutingKeys)
                .UsingConfigConnectionSettings(configuration)
                .Build();

            var dataRetriever = new HttpDataRetriever(new HttpClient());

            var engine = new WebPageDownloaderEngine(
                configuration,
                dataRetriever,
                rmqPublisher,
                rmqConsumer,
                logger);

            engine.Start();

            Console.ReadLine();
        }
    }
}
