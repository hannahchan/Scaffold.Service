namespace Scaffold.Application.Common.Instrumentation
{
    using System.Diagnostics;
    using Scaffold.Application.Common.Constants;

    internal static class ActivityProvider
    {
        private static readonly ActivitySource ActivitySource =
            new ActivitySource(Application.ActivitySource.Name, Application.ActivitySource.Version);

        public static Activity? StartActivity(string name)
        {
            return ActivitySource.StartActivity(name, ActivityKind.Internal);
        }
    }
}
