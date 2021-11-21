namespace Scaffold.Domain.Base;

using System;

public interface IChangeTrackingTimestamps
{
    DateTime CreatedAt { get; }

    DateTime? LastModifiedAt { get; }
}
