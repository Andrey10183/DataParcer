using System.Threading.Tasks;
using LAB.DataScanner.Components.Services.Generators;
using LAB.DataScanner.Components.Services.MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LAB.DataScanner.UrlsGenerator
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
            var logger = loggerFactory.CreateLogger<UrlsGeneratorEngine>();

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            var rmqPublisher = new RmqPublisherBuilder()
                .UsingDefaultConnectionSetting()
                .Build();

            var service = new UrlsGeneratorEngine(
                rmqPublisher,
                configuration,
                logger);

            service.Start();
        }
    }
}
