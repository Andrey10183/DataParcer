using HtmlAgilityPack;
using LAB.DataScanner.Components.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Services.Converters.ConvertStrategies
{
    public class CustomHtmlToJsonConverter : IHtmlToJsonConverter
    {
        private Dictionary<string, string> _expressions;

        public CustomHtmlToJsonConverter(Dictionary<string, string> expressions)
        {
            _expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));

            if (_expressions.Count == 0)
                throw new ArgumentException($"expressions {_expressions} can't be empty");
        }

        public async Task<string> ConvertAsync(string html)
        {
            if (string.IsNullOrEmpty(html))
                throw new ArgumentException($"'{nameof(html)}' cannot be null or empty.", nameof(html));

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            HtmlNodeCollection items;    
            var dataLists = new List<List<string>>();

            foreach (var e in _expressions)
            {
                var line = new List<string>();
                items = htmlDocument.DocumentNode.SelectNodes(e.Value);
                
                if (items != null)
                    foreach (var item in items)
                        line.Add(item?.FirstChild.InnerText);
                dataLists.Add(line);
            }
           
            var result = ListCompression(dataLists, _expressions.Keys.ToList());
            return JsonConvert.SerializeObject(result);
        }

        private List<Dictionary<T, T>> ListCompression<T>(List<List<T>> listToCompress, List<T> Keys)
        {
            var maxLength = listToCompress.Max(x => x.Count);
            var result = new List<Dictionary<T, T>>();
            var listsCount = listToCompress.Count;
            
            for (var i = 0; i < maxLength; i++)
            {
                var dict = new Dictionary<T, T>();
                for (var j = 0; j < Keys.Count; j++)
                    if ( i < listToCompress[j].Count)
                        dict.Add(Keys[j], listToCompress[j][i]);
                    else
                        dict.Add(Keys[j], default);
                result.Add(dict);
            }

            return result;
        }
    }
}
