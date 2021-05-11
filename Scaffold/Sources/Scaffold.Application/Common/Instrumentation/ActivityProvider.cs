namespace Scaffold.Application.Common.Instrumentation
{
    using System;
    using System.Diagnostics;

    public static class ActivityProvider
    {
        public static readonly string ActivitySourceName = typeof(ActivityProvider).Assembly.GetName().Name;

        public static readonly Version ActivitySourceVersion = typeof(ActivityProvider).Assembly.GetName().Version;

        private static readonly ActivitySource ActivitySource = new ActivitySource(ActivitySourceName, ActivitySourceVersion.ToString());

        internal static Activity? StartActivity(string name)
        {
            return ActivitySource.StartActivity(name, ActivityKind.Internal);
        }
    }
}
