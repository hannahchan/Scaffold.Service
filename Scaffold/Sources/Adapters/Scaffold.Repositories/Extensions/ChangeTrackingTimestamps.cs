namespace Scaffold.Repositories.Extensions;

using System;

[Flags]
internal enum ChangeTrackingTimestamps
{
    /// <summary>Used to indicated that a 'CreatedAt' property should be added.</summary>
    CreatedAt = 1,

    /// <summary>Used to indicate that a 'LastModifiedAt' property should be added.</summary>
    LastModifiedAt = 2,

    /// <summary>Used to indicate that a 'DeletedAt' property should be added and that soft deletes should be enabled.</summary>
    DeletedAt = 4,

    /// <summary>Used to indicate that the 'CreatedAt' and 'LastModifiedAt' properties should be added.</summary>
    Default = CreatedAt | LastModifiedAt,

    /// <summary>Used to indicate that all change tracking properties should be added and that soft deletes should be enabled.</summary>
    All = CreatedAt | LastModifiedAt | DeletedAt,
}
