namespace Scaffold.Application.Common.Instrumentation
{
    using System;
    using System.Diagnostics;

    internal static class ActivityExtensions
    {
        public static Activity? InvokeIfNotNullAndIsAllDataRequested(this Activity? activity, Action<Activity> action)
        {
            if (activity != null && activity.IsAllDataRequested)
            {
                action(activity);
            }

            return activity;
        }
    }
}
