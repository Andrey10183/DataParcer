using LAB.DataScanner.Components.Services.Converters.ConvertStrategies;
using LAB.DataScanner.Components.Services.Downloader;
using LAB.DataScanner.Components.Services.MessageBroker;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PublisherExample
{
    class PublisherProgram
    {
        //Publisher test
        //static string exchange = "exampleExchangeTopicDurable"; 
        //static string routingKey1 = "exampleRoutingKey1";
        //static string routingKey2 = "exampleRoutingKey2";
        //static bool exit = false;

        private  static HttpDataRetriever retriever;
        private static int counter = 0;

        static async Task Main(string[] args)
        {
            var result = await Testy();
            Console.WriteLine(result);
            //    var publisherBuild = new RmqPublisherBuilder();
            //    publisherBuild.UsingDefaultConnectionSetting();
            //    publisherBuild.UsingExchange(exchange);
            //    publisherBuild.UsingRoutingKey(routingKey1);
            //    var publisher = publisherBuild.Build();
            //    while (!exit)
            //    {
            //        Console.WriteLine("Input message");
            //        var message = Console.ReadLine();

            //        if (message.ToLower() == "q")
            //        {
            //            exit = true;
            //            continue;
            //        } 
            //        var body = Encoding.UTF8.GetBytes(message);
            //        publisher.Publish(body);
            //    }

            //    publisher.Dispose();

            //retriever test
            //var client = new HttpClient();
            //retriever = new HttpDataRetriever(client);

            //string url = "https://www.kinopoisk.ru/lists/series-top250/?page=3&tab=all";
            //string url1 = "https://yandex.ru";

            //var webPage = await retriever.RetrieveStringAsync(url1);
            //using (StreamWriter sw = new StreamWriter(Path.Combine(@"E:\Download", DateTime.Now.ToString("ddMMyy_hhmmss") + counter.ToString() + ".html")))
            //    await sw.WriteAsync(webPage);

            //Console.WriteLine(webPage);

        }

        //private async string GetWebPage(string url)
        //{ 
        //    return await retriever.RetrieveStringAsync(url);
        //}

        private void CheckConfiguration(object input)
        {
            PropertyInfo[] props = input.GetType().GetProperties();
        }

        private static async Task<string> Testy(string q = "")
        {
            throw new NotImplementedException();
            //using (var client = new HttpClient())
            //{
            //    string url = @"https://jackfinlay.com";

            //    HttpResponseMessage response = await client.GetAsync(url);

            //    string html = await response.Content.ReadAsStringAsync();

            //    // The use of the parameterless constructors will use default settings.
            //    //var converter = new BasicHtmlToJsonCoverter();

            //    //Jsonizer jsonizer = new Jsonizer(parser, serializer);

            //    return await converter.ConvertAsync(html);
            //}
        }
    }
}
