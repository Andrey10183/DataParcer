using LAB.DataScanner.Components.Interfaces;
using LAB.DataScanner.Components.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace LAB.DataScanner.Components.Services.Persisters
{
    public class SimpleDBPersisterEngine
    {
        private IConfiguration _configuration;
        private readonly IRmqConsumer _rmqConsumer;
        private readonly IPersisterDBContext _db;
        private ILogger _logger;
        
        internal ConcurrentQueue<string> _jsonList;

        public SimpleDBPersisterEngine(
            IPersisterDBContext context,
            IConfiguration configuration,
            IRmqConsumer rmqConsumer,
            ILogger<SimpleDBPersisterEngine> logger = null)
        {
            _db = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _rmqConsumer = rmqConsumer ?? throw new ArgumentNullException(nameof(rmqConsumer));
            _logger = logger;

            _jsonList = new ConcurrentQueue<string>();
        }

        public async void Start()
        {
            _logger?.LogInformation("DatabasePersister instance has started!");
            _rmqConsumer.StartListening(OnRecieve);

            Thread queueCheckThread = new Thread(StateCheck);
            queueCheckThread.Start();
        }

        private void OnRecieve(object sender, BasicDeliverEventArgs ea)
        {
            _logger?.LogInformation($"Message recieved from exchange: {ea.Exchange}, routing key: {ea.RoutingKey}");
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _jsonList.Enqueue(message);
            _rmqConsumer.Ack(ea);
        }

        private void StateCheck()
        {
            while (true)
            {
                if (_jsonList.Count > 0 &&
                    _jsonList.TryDequeue(out string currentJson) &&
                    !string.IsNullOrEmpty(currentJson))
                {
                    try
                    {
                        var dataToWrite = JsonConvert.DeserializeObject<PersisterModel[]>(currentJson);

                        if (dataToWrite != null &&
                            dataToWrite.Length > 0)
                        {
                            _db.Data.AddRange(dataToWrite);
                            _db.Save();
                            _logger?.LogInformation("Data successfully saved in Database");
                        }
                    }
                    catch (JsonSerializationException e)
                    {
                        _logger.LogError($"Invalid serialization object. Error message: {e.Message}");
                        throw;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Unexpected exception. Error message: {e.Message}");
                        throw;
                    }                    
                }
            }
        }        
    }
}
