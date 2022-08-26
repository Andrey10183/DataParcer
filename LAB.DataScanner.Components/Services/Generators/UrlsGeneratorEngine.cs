using LAB.DataScanner.Components.Interfaces;
using LAB.DataScanner.Components.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LAB.DataScanner.Components.Services.Generators
{
    public class UrlsGeneratorEngine
    {
        private readonly IRmqPublisher _rmqPublisher;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        private UrlConfiguration _urlConfiguration;
        private BindingConfiguration _bindingConfiguration;

        public UrlsGeneratorEngine(
            IRmqPublisher rmqPublisher,
            IConfiguration configuration,
            ILogger<UrlsGeneratorEngine> logger = null)
        {
            _rmqPublisher = rmqPublisher ?? throw new ArgumentNullException(nameof(rmqPublisher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;

            _bindingConfiguration = new BindingConfiguration();
            _configuration.GetSection(BindingConfiguration.Binding).Bind(_bindingConfiguration);

            _urlConfiguration = new UrlConfiguration();
            _configuration.GetSection(UrlConfiguration.Application).Bind(_urlConfiguration);

            _logger?.LogInformation("UrlGenerator instance initialized successfully!");
        }

        public void Start()
        {
            _logger?.LogInformation("UrlGenerator instance has started!");

            CheckConfigurationValues();

            var rangeOptions = GetRangeOptions(_urlConfiguration.Sequences);

            var urlList = BuildUrlsList(_urlConfiguration.UrlTemplate, rangeOptions);

            Publish(
                urlList,
                _bindingConfiguration.SenderExchange,
                _bindingConfiguration.SenderRoutingKeys);
        }

        private IEnumerable<string> BuildUrlsList(
            string templateUrl,
            List<IEnumerable<int>> rangeOptions)
        {            
            var matches = Regex.Matches(templateUrl, "{[0-9]+}");
            if (matches.Count != rangeOptions.Count)
            {
                _logger?.LogError("Given url template and sequences dosen't match!");
                throw new ArgumentException("Given url template and sequences dosen't match!");
            }
                
            var rangeOptionsCombinatorial = GetCombinatorialSequence(rangeOptions);
            
            foreach (var line in rangeOptionsCombinatorial)
            {
                var sewlectedLine = line.Select(x => x.ToString()).ToArray();
                yield return string.Format(templateUrl, sewlectedLine);
            }
        }

        private void Publish(
            IEnumerable<string> urlsList,
            string exchangeName,
            string[] routingKeys)
        {
            if (urlsList is null)
            {
                _logger?.LogError($"{nameof(urlsList)} can't be null");
                throw new ArgumentNullException($"{nameof(urlsList)}");                
            }

            foreach (var url in urlsList)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    var body = Encoding.UTF8.GetBytes(url);
                    _rmqPublisher.Publish(body, exchangeName, routingKeys);

                    _logger?.LogInformation($"URL: {url} published to exchange: {exchangeName}!");
                }                
            }            
        }

        private List<IEnumerable<int>> GetRangeOptions(string[] sequences)
        {
            var result = new List<IEnumerable<int>>();
            foreach (var item in sequences)
            {
                var limits = item.Split("..");
                if (limits.Length == 2 &&
                    int.TryParse(limits[0], out int loLimit) &&
                    int.TryParse(limits[1], out int hiLimit) &&
                    hiLimit > loLimit)
                {
                    var array = new int[hiLimit - loLimit + 1];
                    var counter = 0;
                    for (var i = loLimit; i <= hiLimit; i++)
                        array[counter++] = i;
                    result.Add(array);
                }
                else
                {
                    _logger?.LogError($"{nameof(sequences)} contain invalid data!");
                    throw new ArgumentException($"{nameof(sequences)} contain invalid data!");
                }                   
            }
            return result;
        }
       
        private IEnumerable<IEnumerable<T>> GetCombinatorialSequence<T>
            (IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
            foreach (IEnumerable<T> sequence in sequences)
                result = from accseq in result from item in sequence select accseq.Concat(new[] { item });
            
            return result;
        }

        private void CheckConfigurationValues()
        {
            if (String.IsNullOrEmpty(_urlConfiguration.UrlTemplate))
            {
                _logger?.LogError($"Invalid configuration! : UrlTemplate null or empty");
                throw new ArgumentException($"Invalid configuration! : UrlTemplate null or empty");
            }

            if (_urlConfiguration.Sequences == null)
            {
                _logger?.LogError($"Invalid configuration! : Sequences is null");
                throw new ArgumentException($"Invalid configuration! : Sequences is null");
            }

            if (String.IsNullOrEmpty(_bindingConfiguration.SenderExchange))
            {
                _logger?.LogError($"Invalid configuration! : SenderExchange null or empty");
                throw new ArgumentException($"Invalid configuration! : SenderExchange null or empty");
            }

            if (_bindingConfiguration.SenderRoutingKeys == null ||
                _bindingConfiguration.SenderRoutingKeys.Length == 0)
            {
                _logger?.LogError($"Invalid configuration! : SenderRoutingKeys null or empty");
                throw new ArgumentException($"Invalid configuration! : SenderRoutingKeys null or empty");
            }
        }
    }
}
