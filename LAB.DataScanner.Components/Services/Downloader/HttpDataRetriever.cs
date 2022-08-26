using LAB.DataScanner.Components.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Services.Downloader
{
    public class HttpDataRetriever : IDataRetriever, IDisposable
    {
        private HttpClient _httpClient;

        public HttpDataRetriever(HttpClient httpClient) =>
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public void Dispose() =>
            _httpClient.Dispose();

        public async Task<byte[]> RetrieveBytesAsync(string uri)
        {
            if (!UrlValid(uri))
                throw new ArgumentException("Invalid uri", nameof(uri));

            var responseBody = await _httpClient.GetAsync(uri);

            if (responseBody.IsSuccessStatusCode)
            {
                var responseArray = await responseBody.Content.ReadAsByteArrayAsync();
                return responseArray;
            }
            else
                return null;
        }

        public async Task<string> RetrieveStringAsync(string uri)
        {
            if (!UrlValid(uri))
                throw new ArgumentException("Invalid uri", nameof(uri));

            var responseBody = await _httpClient.GetAsync(uri);

            if (responseBody.IsSuccessStatusCode)
            {
                var responseArray = await responseBody.Content.ReadAsStringAsync();
                return responseArray;
            }
            else
                return null;            
        }

        private bool UrlValid(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
