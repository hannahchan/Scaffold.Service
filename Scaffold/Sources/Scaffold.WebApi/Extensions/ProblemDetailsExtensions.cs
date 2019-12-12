namespace Scaffold.WebApi.Extensions
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;

    public static class ProblemDetailsExtensions
    {
        public static ProblemDetails AddW3cTraceId(this ProblemDetails details)
        {
            if (Activity.Current is Activity activity && activity.IdFormat == ActivityIdFormat.W3C)
            {
                details.Extensions["traceId"] = activity.TraceId.ToString();
            }

            return details;
        }
    }
}
