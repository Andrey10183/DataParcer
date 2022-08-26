using LAB.DataScanner.Components.Models;
using LAB.DataScanner.Components.Services.Converters;
using LAB.DataScanner.Components.Services.MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
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
            var logger = loggerFactory.CreateLogger<HtmlToJsonConverterEngine>();

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

            var converterEngine = new HtmlToJsonConverterEngine(
                configuration,
                rmqPublisher,
                rmqConsumer,
                logger);

            converterEngine.Start();

            Console.ReadLine();
        }
    }
}
