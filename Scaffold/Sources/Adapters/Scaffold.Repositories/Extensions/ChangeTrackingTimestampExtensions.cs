namespace Scaffold.Repositories.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal static class ChangeTrackingTimestampExtensions
    {
        public static ModelBuilder AddChangeTrackingTimestamps(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.FindProperty(PropertyName.CreatedAt) == null)
                {
                    entity.AddProperty(PropertyName.CreatedAt, typeof(DateTime));
                }

                if (entity.FindProperty(PropertyName.LastModifiedAt) == null)
                {
                    entity.AddProperty(PropertyName.LastModifiedAt, typeof(DateTime?));
                }
            }

            return modelBuilder;
        }

        public static ChangeTracker UpdateChangeTrackingTimestamps(this ChangeTracker changeTracker, DateTime timestamp)
        {
            return changeTracker
                .UpdateTimestampsOnAdded(timestamp)
                .UpdateTimestampsOnModified(timestamp);
        }

        private static ChangeTracker UpdateTimestampsOnAdded(this ChangeTracker changeTracker, DateTime timestamp)
        {
            IEnumerable<EntityEntry> entries = changeTracker.Entries()
                .Where(entry => entry.State == EntityState.Added)
                .Where(entry => entry.Metadata.FindProperty(PropertyName.CreatedAt) != null)
                .Where(entry => entry.Metadata.FindProperty(PropertyName.CreatedAt).ClrType == typeof(DateTime));

            foreach (EntityEntry entry in entries)
            {
                entry.Property(PropertyName.CreatedAt).CurrentValue = timestamp;

                IProperty? lastModifiedAt = entry.Metadata.FindProperty(PropertyName.LastModifiedAt);

                if (lastModifiedAt is null)
                {
                    continue;
                }

                if (lastModifiedAt.ClrType == typeof(DateTime))
                {
                    entry.Property(PropertyName.LastModifiedAt).CurrentValue = timestamp;
                }

                if (lastModifiedAt.ClrType == typeof(DateTime?))
                {
                    entry.Property(PropertyName.LastModifiedAt).CurrentValue = null;
                }
            }

            return changeTracker;
        }

        private static ChangeTracker UpdateTimestampsOnModified(this ChangeTracker changeTracker, DateTime timestamp)
        {
            IEnumerable<EntityEntry> entries = changeTracker.Entries()
                .Where(entry => entry.State == EntityState.Modified)
                .Where(entry => entry.Metadata.FindProperty(PropertyName.LastModifiedAt) != null)
                .Where(entry =>
                {
                    Type lastModifiedAtType = entry.Metadata.FindProperty(PropertyName.LastModifiedAt).ClrType;
                    return lastModifiedAtType == typeof(DateTime) || lastModifiedAtType == typeof(DateTime?);
                });

            foreach (EntityEntry entry in entries)
            {
                entry.Property(PropertyName.LastModifiedAt).CurrentValue = timestamp;
            }

            return changeTracker;
        }
    }
}
