using Cell.Blazor.Data.Interface;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cell.Blazor.Data.Class
{
    public class HttpHandlerBase : IHttpHandler
    {
        public HttpHandlerBase(HttpClient _http) => _client = _http;

        private HttpClient _client { get; set; }

        public virtual HttpClient GetClient() => _client ??= new HttpClient();

        public virtual async Task<HttpResponseMessage> SendRequest(
            HttpRequestMessage data)
        {
            return await Task.FromResult((HttpResponseMessage)null);
        }
    }
}