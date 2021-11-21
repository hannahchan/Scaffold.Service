namespace Scaffold.Application.Common.Interfaces;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public interface IExampleHttpClient
{
    Task<HttpResponseMessage> Get(string path, CancellationToken cancellationToken = default);
}
