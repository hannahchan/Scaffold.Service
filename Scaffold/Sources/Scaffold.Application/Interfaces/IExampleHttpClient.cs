namespace Scaffold.Application.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IExampleHttpClient
    {
        Task<HttpResponseMessage> Get(string path);
    }
}
