namespace Scaffold.Repositories.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

internal static class ChangeTrackingTimestampExtensions
{
    public static ModelBuilder AddChangeTrackingTimestamps(this ModelBuilder modelBuilder, ChangeTrackingTimestamps properties = ChangeTrackingTimestamps.Default)
    {
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
        {
            if (properties.HasFlag(ChangeTrackingTimestamps.CreatedAt) && entity.FindProperty(PropertyName.CreatedAt) == null)
            {
                entity.AddProperty(PropertyName.CreatedAt, typeof(DateTime));
            }

            if (properties.HasFlag(ChangeTrackingTimestamps.LastModifiedAt) && entity.FindProperty(PropertyName.LastModifiedAt) == null)
            {
                entity.AddProperty(PropertyName.LastModifiedAt, typeof(DateTime?));
            }

            if (properties.HasFlag(ChangeTrackingTimestamps.DeletedAt) && entity.FindProperty(PropertyName.DeletedAt) == null)
            {
                entity.AddProperty(PropertyName.DeletedAt, typeof(DateTime?));
            }

            if (properties.HasFlag(ChangeTrackingTimestamps.DeletedAt))
            {
                entity.AddSoftDeleteQueryFilter();
            }
        }

        return modelBuilder;
    }

    public static ChangeTracker UpdateChangeTrackingTimestamps(this ChangeTracker changeTracker, DateTime timestamp)
    {
        return changeTracker
            .UpdateTimestampsOnAdded(timestamp)
            .UpdateTimestampsOnModified(timestamp)
            .UpdateTimestampsOnDeleted(timestamp);
    }

    private static IMutableEntityType AddSoftDeleteQueryFilter(this IMutableEntityType entity)
    {
        IMutableProperty deletedAt = entity.GetProperty(PropertyName.DeletedAt);

        if (deletedAt.ClrType == typeof(DateTime))
        {
            ParameterExpression parameter = Expression.Parameter(entity.ClrType, "entity");

            MethodCallExpression methodCall = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new[] { typeof(DateTime) },
                parameter,
                Expression.Constant(PropertyName.DeletedAt));

            LambdaExpression lambda = Expression.Lambda(Expression.Equal(methodCall, Expression.Default(typeof(DateTime))), parameter);

            entity.SetQueryFilter(lambda);
        }

        if (deletedAt.ClrType == typeof(DateTime?))
        {
            ParameterExpression parameter = Expression.Parameter(entity.ClrType, "entity");

            MethodCallExpression methodCall = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new[] { typeof(DateTime?) },
                parameter,
                Expression.Constant(PropertyName.DeletedAt));

            LambdaExpression lambda = Expression.Lambda(Expression.Equal(methodCall, Expression.Constant(null)), parameter);

            entity.SetQueryFilter(lambda);
        }

        return entity;
    }

    private static ChangeTracker UpdateTimestampsOnAdded(this ChangeTracker changeTracker, DateTime timestamp)
    {
        IEnumerable<EntityEntry> entries = changeTracker.Entries()
            .Where(entry => entry.State == EntityState.Added)
            .Where(entry => entry.Metadata.FindProperty(PropertyName.CreatedAt) != null || entry.Metadata.FindProperty(PropertyName.LastModifiedAt) != null);

        foreach (EntityEntry entry in entries)
        {
            IProperty? createdAt = entry.Metadata.FindProperty(PropertyName.CreatedAt);

            if (createdAt != null && createdAt.ClrType == typeof(DateTime))
            {
                entry.Property(PropertyName.CreatedAt).CurrentValue = timestamp;
            }

            IProperty? lastModifiedAt = entry.Metadata.FindProperty(PropertyName.LastModifiedAt);

            if (lastModifiedAt != null)
            {
                if (lastModifiedAt.ClrType == typeof(DateTime))
                {
                    entry.Property(PropertyName.LastModifiedAt).CurrentValue = timestamp;
                }

                if (lastModifiedAt.ClrType == typeof(DateTime?))
                {
                    entry.Property(PropertyName.LastModifiedAt).CurrentValue = null;
                }
            }
        }

        return changeTracker;
    }

    private static ChangeTracker UpdateTimestampsOnModified(this ChangeTracker changeTracker, DateTime timestamp)
    {
        IEnumerable<EntityEntry> entries = changeTracker.Entries()
            .Where(entry => entry.State == EntityState.Modified)
            .Where(entry =>
            {
                IProperty? lastModifiedProperty = entry.Metadata.FindProperty(PropertyName.LastModifiedAt);
                return lastModifiedProperty != null && (lastModifiedProperty.ClrType == typeof(DateTime) || lastModifiedProperty.ClrType == typeof(DateTime?));
            });

        foreach (EntityEntry entry in entries)
        {
            entry.Property(PropertyName.LastModifiedAt).CurrentValue = timestamp;
        }

        return changeTracker;
    }

    private static ChangeTracker UpdateTimestampsOnDeleted(this ChangeTracker changeTracker, DateTime timestamp)
    {
        IEnumerable<EntityEntry> entries = changeTracker.Entries()
            .Where(entry => entry.State == EntityState.Deleted)
            .Where(entry =>
            {
                IProperty? deletedAtProperty = entry.Metadata.FindProperty(PropertyName.DeletedAt);
                return deletedAtProperty != null && (deletedAtProperty.ClrType == typeof(DateTime) || deletedAtProperty.ClrType == typeof(DateTime?));
            });

        foreach (EntityEntry entry in entries)
        {
            entry.Property(PropertyName.DeletedAt).CurrentValue = timestamp;
            entry.State = EntityState.Modified;
        }

        return changeTracker;
    }
}
