namespace Scaffold.Application.Behavior
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Scaffold.Application.Context;

    public class RequestIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response = await next();

            if (request is ApplicationRequest && response is ApplicationResponse)
            {
                ApplicationRequest applicationRequest = request as ApplicationRequest;
                ApplicationResponse applicationResponse = response as ApplicationResponse;

                applicationResponse.RequestId = applicationRequest.RequestId;
            }

            return response;
        }
    }
}
