namespace Scaffold.Application.Common.Instrumentation;

using System;
using System.Diagnostics;

internal static class ActivityExtensions
{
    public static string? GetSpanId(this Activity activity)
    {
        return activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.Id,
            ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
            _ => null,
        };
    }

    public static string? GetTraceId(this Activity activity)
    {
        return activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.RootId,
            ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
            _ => null,
        };
    }

    public static string? GetParentId(this Activity activity)
    {
        return activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.ParentId,
            ActivityIdFormat.W3C => activity.ParentSpanId.ToHexString(),
            _ => null,
        };
    }

    public static Activity? InvokeIfNotNullAndIsAllDataRequested(this Activity? activity, Action<Activity> action)
    {
        if (activity != null && activity.IsAllDataRequested)
        {
            action(activity);
        }

        return activity;
    }
}
