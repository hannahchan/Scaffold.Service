namespace Scaffold.Application.Interfaces
{
    public interface IRequestTracingService
    {
        string CorrelationId { get; set; }
    }
}
