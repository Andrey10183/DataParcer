using LAB.DataScanner.Components.Interfaces;
using LAB.DataScanner.Components.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Threading;

namespace LAB.DataScanner.Components.Services.Downloader
{
    public class WebPageDownloaderEngine
    {
        private IConfiguration _configuration;
        private readonly IDataRetriever _dataRetriever;
        private readonly IRmqPublisher _rmqPublisher;
        private readonly IRmqConsumer _rmqConsumer;
        private ILogger _logger;
        internal ConcurrentQueue<string> _urlList;
        private BindingConfiguration _bindingConfiguration;

        public WebPageDownloaderEngine(
            IConfiguration configuration,
            IDataRetriever dataRetriever,
            IRmqPublisher rmqPublisher,
            IRmqConsumer rmqConsumer,
            ILogger<WebPageDownloaderEngine> logger = null)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dataRetriever = dataRetriever ?? throw new ArgumentNullException(nameof(dataRetriever));
            _rmqPublisher = rmqPublisher ?? throw new ArgumentNullException(nameof(rmqPublisher));
            _rmqConsumer = rmqConsumer ?? throw new ArgumentNullException(nameof(rmqConsumer));
            _logger = logger;

            _urlList = new ConcurrentQueue<string>();

            _bindingConfiguration = new BindingConfiguration();
            _configuration.GetSection(BindingConfiguration.Binding).Bind(_bindingConfiguration);

            CheckConfiguration(_bindingConfiguration);
            
            _logger?.LogInformation("WebPageDownloader instance initialized successfully!");
        }

        public async void Start()
        {
            _logger?.LogInformation("WebPageDownloader instance has started!");
            _rmqConsumer.StartListening(OnRecieve);

            Thread queueCheckThread = new Thread(StateCheck);
            queueCheckThread.Start();
        }

        private void OnRecieve(object sender, BasicDeliverEventArgs ea)
        {
            _logger?.LogInformation($"Message recieved from exchange: {ea.Exchange}, routing key: {ea.RoutingKey}");
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _urlList.Enqueue(message);
            _rmqConsumer.Ack(ea);
        }

        private void CheckConfiguration(object input)
        {
            PropertyInfo[] props = input.GetType().GetProperties();
            foreach (var prop in props)
                if (prop.GetValue(input, null) is null)
                {
                    _logger?.LogError($"Execution aborted! Invalid configuration parameter {prop.Name}");
                    throw new ArgumentNullException($"Execution aborted! Invalid configuration parameter {prop.Name}");
                }                    
        }

        private void StateCheck()
        {
            while (true)
            {
                if (_urlList.Count > 0 &&
                    _urlList.TryDequeue(out string currentUri))
                {
                    if (UrlValid(currentUri))
                    {
                        var webPage = _dataRetriever.RetrieveBytesAsync(currentUri).GetAwaiter().GetResult();
                        if (webPage != null)
                        {
                            _rmqPublisher.Publish(webPage, _bindingConfiguration.SenderExchange, _bindingConfiguration.SenderRoutingKeys);
                            _logger.LogInformation($"Page downloaded and published on exchange: {_bindingConfiguration.SenderExchange}");
                        }                            
                    }
                    else 
                    {
                        _logger?.LogWarning($"Invalid url: {currentUri}. Download operation skipped!");
                    }                        
                }
            }
        }

        private bool UrlValid(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out Uri uri) && 
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
