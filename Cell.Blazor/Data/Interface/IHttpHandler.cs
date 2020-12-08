using System.Net.Http;
using System.Threading.Tasks;

namespace Cell.Blazor.Data.Interface
{
    public interface IHttpHandler
    {
        HttpClient GetClient();

        Task<HttpResponseMessage> SendRequest(HttpRequestMessage data);
    }
}