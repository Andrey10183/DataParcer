using LAB.DataScanner.Components.Interfaces;
using LAB.DataScanner.Components.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.XPath;

namespace LAB.DataScanner.Components.Services.Converters
{
    public class HtmlToJsonConverterEngine
    {
        private readonly IConfiguration _configuration;
        private IHtmlToJsonConverter _converter;
        private readonly IRmqPublisher _rmqPublisher;
        private readonly IRmqConsumer _rmqConsumer;
        internal ConcurrentQueue<string> _htmlPagesList;
        private BindingConfiguration _bindingConfiguration;
        private ConverterConfiguration _converterConfiguration;
        private ILogger _logger;

        public HtmlToJsonConverterEngine(
            IConfiguration configuration,
            IRmqPublisher rmqPublisher,
            IRmqConsumer rmqConsumer,
            ILogger<HtmlToJsonConverterEngine> logger = null)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _rmqPublisher = rmqPublisher ?? throw new ArgumentNullException(nameof(rmqPublisher));
            _rmqConsumer = rmqConsumer ?? throw new ArgumentNullException(nameof(rmqConsumer));
            _logger = logger;

            _htmlPagesList = new ConcurrentQueue<string>();

            _bindingConfiguration = new BindingConfiguration();
            _configuration.GetSection(BindingConfiguration.Binding).Bind(_bindingConfiguration);

            _converterConfiguration = new ConverterConfiguration();
            _configuration.GetSection(ConverterConfiguration.ConverterConfig).Bind(_converterConfiguration);

            _converter = GetConverterInstance(
                _converterConfiguration.HtmlFragmentStrategy,
                _converterConfiguration.HtmlFragmentExpressions) ??  throw new ArgumentNullException($"there is no such strategy name {_converterConfiguration.HtmlFragmentStrategy}");

            _logger?.LogInformation("HtmlToJsonConverter instance initialized successfully!");
        }

        public void SetConverter(IHtmlToJsonConverter converter) =>
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));

        public async Task<string> Convert(string html) =>
            await _converter.ConvertAsync(html);

        public void Start()
        {
            _logger?.LogInformation("HtmlToJsonConverter instance has started!");
            _rmqConsumer.StartListening(OnRecieve);

            Thread queueCheckThread = new Thread(StateCheck);
            queueCheckThread.Start();
        }

        private void OnRecieve(object sender, BasicDeliverEventArgs ea)
        {
            _logger?.LogInformation($"Message recieved from exchange: {ea.Exchange}, routing key: {ea.RoutingKey}");
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _htmlPagesList.Enqueue(message);
            _rmqConsumer.Ack(ea);
        }

        private async void StateCheck()
        {
            while (true)
            {
                if (_htmlPagesList.Count > 0 &&
                    _htmlPagesList.TryDequeue(out string currentHtml) &&
                    !string.IsNullOrEmpty(currentHtml))
                {
                    try
                    {
                        var jsonData = await Convert(currentHtml);

                        if (jsonData != null)
                        {
                            var body = Encoding.UTF8.GetBytes(jsonData);
                            _rmqPublisher.Publish(body, _bindingConfiguration.SenderExchange, _bindingConfiguration.SenderRoutingKeys);
                            _logger.LogInformation($"Html page converted and published on exchange: {_bindingConfiguration.SenderExchange}");
                        }

                    }
                    catch (XPathException e)
                    {
                        _logger?.LogError($"Invalid expression. Error message {e.Message}");
                        throw;
                    }
                    catch (Exception e)
                    {
                        _logger?.LogError($"Unexpected excpetion. Error message {e.Message}");
                        throw;
                    }                   
                }
            }
        }

        private IHtmlToJsonConverter GetConverterInstance(string strNamesapace, Dictionary<string, string> expressions)
        {
            Type t = Type.GetType(strNamesapace);
            if (t == null)
                return null;
            else
                return Activator.CreateInstance(t, new object[] { expressions }) as IHtmlToJsonConverter;
        }
    }
}
