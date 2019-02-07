namespace Scaffold.WebApi.Services
{
    using Scaffold.Application.Interfaces;

    public class RequestIdService : IRequestIdService
    {
        public string RequestId { get; set; }
    }
}
