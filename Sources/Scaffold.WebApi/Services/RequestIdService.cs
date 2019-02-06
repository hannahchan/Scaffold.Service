namespace Scaffold.WebApi.Middleware
{
    using Scaffold.Application.Interfaces;

    public class RequestIdService : IRequestIdService
    {
        public string RequestId { get; set; }
    }
}
