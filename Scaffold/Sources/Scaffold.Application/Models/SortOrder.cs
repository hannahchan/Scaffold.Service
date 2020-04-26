namespace Scaffold.Application.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using Scaffold.Application.Exceptions;

    public class SortOrder<T> : IReadOnlyList<(string PropertyName, bool Descending)>
    {
        private static readonly PropertyInfo[] Properties = typeof(T).GetProperties();

        private readonly IImmutableList<(string PropertyName, bool Descending)> sortOrder;

        private SortOrder(IImmutableList<(string PropertyName, bool Descending)> sortOrder)
        {
            this.sortOrder = sortOrder;
        }

        public int Count => this.sortOrder.Count;

        public (string PropertyName, bool Descending) this[int index]
        {
            get => this.sortOrder[index];
        }

        public static SortOrder<T> OrderBy(string propertyName)
        {
            return new SortOrder<T>(ImmutableArray<(string PropertyName, bool Descending)>.Empty)
                .Add(propertyName, false);
        }

        public static SortOrder<T> OrderByDescending(string propertyName)
        {
            return new SortOrder<T>(ImmutableArray<(string PropertyName, bool Descending)>.Empty)
                .Add(propertyName, true);
        }

        public SortOrder<T> ThenBy(string propertyName)
        {
            return this.Add(propertyName, false);
        }

        public SortOrder<T> ThenByDescending(string propertyName)
        {
            return this.Add(propertyName, true);
        }

        public IEnumerator<(string PropertyName, bool Descending)> GetEnumerator()
        {
            return this.sortOrder.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private SortOrder<T> Add(string propertyName, bool descending)
        {
            this.ValidateProperty(propertyName);
            return new SortOrder<T>(this.sortOrder.Add((propertyName, descending)));
        }

        private void ValidateProperty(string propertyName)
        {
            PropertyInfo property = Properties.SingleOrDefault(x => string.Equals(x.Name, propertyName, StringComparison.Ordinal));

            if (property is null)
            {
                throw new PropertyNotFoundException(propertyName, typeof(T).Name);
            }

            if (property.PropertyType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IComparable<>)))
            {
                return;
            }

            if (typeof(IComparable).IsAssignableFrom(property.PropertyType))
            {
                return;
            }

            throw new PropertyNotComparableException(propertyName);
        }
    }
}
