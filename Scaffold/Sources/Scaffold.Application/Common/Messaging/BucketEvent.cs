namespace Scaffold.Application.Common.Messaging;

using System;
using System.Diagnostics;
using MediatR;
using Scaffold.Application.Common.Instrumentation;

internal abstract record BucketEvent(string Type, string Description) : IAuditableEvent, INotification
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public string? TraceId { get; } = Activity.Current?.GetTraceId();
}

internal record BucketAddedEvent(int BucketId) : BucketEvent(
    Type: "BucketAdded",
    Description: $"Added Bucket {BucketId}");

internal record BucketsRetrievedEvent(int[] BucketIds) : BucketEvent(
    Type: "BucketsRetrieved",
    Description: $"Retrieved {BucketIds.Length} Bucket/s");

internal record BucketRetrievedEvent(int BucketId) : BucketEvent(
    Type: "BucketRetrieved",
    Description: $"Retrieved Bucket {BucketId}");

internal record BucketRemovedEvent(int BucketId) : BucketEvent(
    Type: "BucketRemoved",
    Description: $"Removed Bucket {BucketId}");

internal record BucketUpdatedEvent(int BucketId) : BucketEvent(
    Type: "BucketUpdated",
    Description: $"Updated Bucket {BucketId}");

internal record ItemAddedEvent(int BucketId, int ItemId) : BucketEvent(
    Type: "ItemAdded",
    Description: $"Added Item {ItemId} to Bucket {BucketId}");

internal record ItemsRetrievedEvent(int BucketId, int[] ItemIds) : BucketEvent(
    Type: "ItemsRetrieved",
    Description: $"Retrieved {ItemIds.Length} Item/s from Bucket {BucketId}");

internal record ItemRetrievedEvent(int BucketId, int ItemId) : BucketEvent(
    Type: "ItemRetrieved",
    Description: $"Retrieved Item {ItemId} from Bucket {BucketId}");

internal record ItemRemovedEvent(int BucketId, int ItemId) : BucketEvent(
    Type: "ItemRemoved",
    Description: $"Removed Item {ItemId} from Bucket {BucketId}");

internal record ItemUpdatedEvent(int BucketId, int ItemId) : BucketEvent(
    Type: "ItemUpdated",
    Description: $"Updated Item {ItemId} in Bucket {BucketId}");
