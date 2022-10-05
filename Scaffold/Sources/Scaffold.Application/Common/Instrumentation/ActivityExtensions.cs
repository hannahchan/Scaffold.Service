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

    public static Activity? InvokeIfRecording(this Activity? activity, Action<Activity> action)
    {
        if (activity != null && activity.IsAllDataRequested)
        {
            action(activity);
        }

        return activity;
    }

    public static Activity RecordException(this Activity activity, Exception exception)
    {
        // Trace Semantic Conventions
        // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/README.md
        ActivityTagsCollection tagsCollection = new ActivityTagsCollection
        {
            { "exception.type", exception.GetType().FullName },
            { "exception.message", exception.Message },
            { "exception.stacktrace", exception.ToString() },
        };

        return activity.AddEvent(new ActivityEvent("exception", default, tagsCollection));
    }
}
