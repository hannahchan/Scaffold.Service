namespace Scaffold.Repositories.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal static class ChangeTrackerExtensions
    {
        public static ChangeTracker UpdateChangeTimestamps(this ChangeTracker changeTracker, DateTime timestamp)
        {
            return changeTracker
                .UpdateTimestampsOnAdded(timestamp)
                .UpdateTimestampsOnModified(timestamp);
        }

        private static ChangeTracker UpdateTimestampsOnAdded(this ChangeTracker changeTracker, DateTime timestamp)
        {
            IEnumerable<EntityEntry> entries = changeTracker.Entries()
                .Where(entry => entry.State == EntityState.Added)
                .Where(entry => entry.Metadata.FindProperty(PropertyNames.CreatedAt) != null)
                .Where(entry => entry.Metadata.FindProperty(PropertyNames.CreatedAt).ClrType == typeof(DateTime));

            foreach (EntityEntry entry in entries)
            {
                entry.Property(PropertyNames.CreatedAt).CurrentValue = timestamp;

                IProperty? lastModifiedAt = entry.Metadata.FindProperty(PropertyNames.LastModifiedAt);

                if (lastModifiedAt is null)
                {
                    continue;
                }

                if (lastModifiedAt.ClrType == typeof(DateTime))
                {
                    entry.Property(PropertyNames.LastModifiedAt).CurrentValue = timestamp;
                }

                if (lastModifiedAt.ClrType == typeof(DateTime?))
                {
                    entry.Property(PropertyNames.LastModifiedAt).CurrentValue = null;
                }
            }

            return changeTracker;
        }

        private static ChangeTracker UpdateTimestampsOnModified(this ChangeTracker changeTracker, DateTime timestamp)
        {
            IEnumerable<EntityEntry> entries = changeTracker.Entries()
                .Where(entry => entry.State == EntityState.Modified)
                .Where(entry => entry.Metadata.FindProperty(PropertyNames.LastModifiedAt) != null)
                .Where(entry =>
                {
                    Type lastModifiedAtType = entry.Metadata.FindProperty(PropertyNames.LastModifiedAt).ClrType;
                    return lastModifiedAtType == typeof(DateTime) || lastModifiedAtType == typeof(DateTime?);
                });

            foreach (EntityEntry entry in entries)
            {
                entry.Property(PropertyNames.LastModifiedAt).CurrentValue = timestamp;
            }

            return changeTracker;
        }
    }
}
