using LAB.DataScanner.Components.Services.Downloader;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Tests.Unit.Services.WebPadeDownloader
{
    public class HttpDataRetrieverTests
    {
        [Test]
        public void Constructor_NullInputParan_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new HttpDataRetriever(null));

        [Test, Sequential]
        public void RetrieveBytesAsync_InvalidUrl_ThrowsArgumentException([Values(null, "", "abc")] string url)
        {
            var client = new HttpClient();
            var dtatretriever = new HttpDataRetriever(client);
            Assert.ThrowsAsync<ArgumentException>(async () => await dtatretriever.RetrieveBytesAsync(url));
        }

        [Test, Sequential]
        public void RetrieveStringAsync_InvalidUrl_ThrowsArgumentException([Values(null, "", "abc")] string url)
        {
            var client = new HttpClient();
            var dtatretriever = new HttpDataRetriever(client);
            Assert.ThrowsAsync<ArgumentException>(async () => await dtatretriever.RetrieveStringAsync(url));
        }

        [Test]
        public async Task RetrieveStringAsync_ValidUrl_Sucssess()
        {
            var expected = "test response from requested url";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expected)
            };

            var MessageHandlerMock = new MockHttpMessageHandler(responseMessage);
            var client = new HttpClient(MessageHandlerMock);
            var retriever = new HttpDataRetriever(client);
            var result = await retriever.RetrieveStringAsync("https://yandex.ru");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task RetrieveBytesAsync_ValidUrl_Sucssess()
        {
            var expected = new byte[] { 100, 101, 102, 103 };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(expected)
            };

            var MessageHandlerMock = new MockHttpMessageHandler(responseMessage);
            var client = new HttpClient(MessageHandlerMock);
            var retriever = new HttpDataRetriever(client);
            var result = await retriever.RetrieveBytesAsync("https://yandex.ru");
            Assert.AreEqual(expected, result);
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage _responceMessage;

        public MockHttpMessageHandler(HttpResponseMessage responceMessage) =>
            _responceMessage = responceMessage;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            await Task.FromResult(_responceMessage);
    }
}
