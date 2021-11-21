namespace Scaffold.Application.Common.Messaging;

using System;
using System.Diagnostics;
using MediatR;
using Scaffold.Application.Common.Instrumentation;

internal abstract record BucketEvent(string? Source, string Type, string Description) : IAuditableEvent, INotification
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public string? TraceId { get; } = Activity.Current?.GetTraceId();
}

internal record BucketAddedEvent<TSource>(int BucketId) : BucketEvent(
    Source: typeof(TSource).FullName,
    Type: "BucketAdded",
    Description: $"Added Bucket {BucketId}");

internal record BucketsRetrievedEvent<TSource>(int[] BucketIds) : BucketEvent(
    Source: typeof(TSource).FullName,
    Type: "BucketsRetrieved",
    Description: $"Retrieved {BucketIds.Length} Bucket/s");

internal record BucketRetrievedEvent<TSource>(int BucketId) : BucketEvent(
    Source: typeof(TSource).FullName,
    Type: "BucketRetrieved",
    Description: $"Retrieved Bucket {BucketId}");

internal record BucketRemovedEvent<TSource>(int BucketId) : BucketEvent(
    Source: typeof(TSource).FullName,
    Type: "BucketRemoved",
    Description: $"Removed Bucket {BucketId}");

internal record BucketUpdatedEvent<TSource>(int BucketId) : BucketEvent(
    Source: typeof(TSource).FullName,
    Type: "BucketUpdated",
    Description: $"Updated Bucket {BucketId}");

internal record ItemAddedEvent<TSource>(int BucketId, int ItemId) : BucketEvent(
    Source: typeof(TSource).FullName,
    Type: "ItemAdded",
    Description: $"Added Item {ItemId} to Bucket {BucketId}");

internal record ItemsRetrievedEvent<TSource>(int BucketId, int[] ItemIds) : BucketEvent(
    Source: typeof(TSource).FullName,
    Type: "ItemsRetrieved",
    Description: $"Retrieved {ItemIds.Length} Item/s from Bucket {BucketId}");

internal record ItemRetrievedEvent<TSource>(int BucketId, int ItemId) : BucketEvent(
    Source: typeof(TSource).FullName,
    Type: "ItemRetrieved",
    Description: $"Retrieved Item {ItemId} from Bucket {BucketId}");

internal record ItemRemovedEvent<TSource>(int BucketId, int ItemId) : BucketEvent(
    Source: typeof(TSource).FullName,
    Type: "ItemRemoved",
    Description: $"Removed Item {ItemId} from Bucket {BucketId}");

internal record ItemUpdatedEvent<TSource>(int BucketId, int ItemId) : BucketEvent(
    Source: typeof(TSource).FullName,
    Type: "ItemUpdated",
    Description: $"Updated Item {ItemId} in Bucket {BucketId}");
