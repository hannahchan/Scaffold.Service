namespace Scaffold.Application.Common.Messaging;

using System;
using System.Diagnostics;
using MediatR;
using Scaffold.Application.Common.Instrumentation;

internal abstract record BucketEvent(Type Source, string Type, string Description) : IAuditableEvent, INotification
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public string? TraceId { get; } = Activity.Current?.GetTraceId();
}

internal record BucketAddedEvent(Type Source, int BucketId) : BucketEvent(
    Source: Source,
    Type: "BucketAdded",
    Description: $"Added Bucket {BucketId}");

internal record BucketsRetrievedEvent(Type Source, int[] BucketIds) : BucketEvent(
    Source: Source,
    Type: "BucketsRetrieved",
    Description: $"Retrieved {BucketIds.Length} Bucket/s");

internal record BucketRetrievedEvent(Type Source, int BucketId) : BucketEvent(
    Source: Source,
    Type: "BucketRetrieved",
    Description: $"Retrieved Bucket {BucketId}");

internal record BucketRemovedEvent(Type Source, int BucketId) : BucketEvent(
    Source: Source,
    Type: "BucketRemoved",
    Description: $"Removed Bucket {BucketId}");

internal record BucketUpdatedEvent(Type Source, int BucketId) : BucketEvent(
    Source: Source,
    Type: "BucketUpdated",
    Description: $"Updated Bucket {BucketId}");

internal record ItemAddedEvent(Type Source, int BucketId, int ItemId) : BucketEvent(
    Source: Source,
    Type: "ItemAdded",
    Description: $"Added Item {ItemId} to Bucket {BucketId}");

internal record ItemsRetrievedEvent(Type Source, int BucketId, int[] ItemIds) : BucketEvent(
    Source: Source,
    Type: "ItemsRetrieved",
    Description: $"Retrieved {ItemIds.Length} Item/s from Bucket {BucketId}");

internal record ItemRetrievedEvent(Type Source, int BucketId, int ItemId) : BucketEvent(
    Source: Source,
    Type: "ItemRetrieved",
    Description: $"Retrieved Item {ItemId} from Bucket {BucketId}");

internal record ItemRemovedEvent(Type Source, int BucketId, int ItemId) : BucketEvent(
    Source: Source,
    Type: "ItemRemoved",
    Description: $"Removed Item {ItemId} from Bucket {BucketId}");

internal record ItemUpdatedEvent(Type Source, int BucketId, int ItemId) : BucketEvent(
    Source: Source,
    Type: "ItemUpdated",
    Description: $"Updated Item {ItemId} in Bucket {BucketId}");
