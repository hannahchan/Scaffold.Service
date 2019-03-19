namespace Scaffold.WebApi.Services
{
    using Scaffold.Application.Interfaces;

    public class RequestTracingService : IRequestTracingService
    {
        public string CorrelationId { get; set; } = null;
    }
}
