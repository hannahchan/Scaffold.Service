namespace Scaffold.Application.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Scaffold.Application.Exceptions;

    public class Ordering<T> : IList<OrderBy>
    {
        private static readonly PropertyInfo[] Properties = typeof(T).GetProperties();

        private readonly IList<OrderBy> ordering = new List<OrderBy>();

        public int Count => this.ordering.Count;

        public bool IsReadOnly => this.ordering.IsReadOnly;

        public OrderBy this[int index]
        {
            get => this.ordering[index];

            set
            {
                this.ValidateProperty(value);
                this.ordering[index] = value;
            }
        }

        public void Add(OrderBy item)
        {
            this.ValidateProperty(item);
            this.ordering.Add(item);
        }

        public void Clear()
        {
            this.ordering.Clear();
        }

        public bool Contains(OrderBy item)
        {
            return this.ordering.Contains(item);
        }

        public void CopyTo(OrderBy[] array, int arrayIndex)
        {
            this.ordering.CopyTo(array, arrayIndex);
        }

        public IEnumerator<OrderBy> GetEnumerator()
        {
            return this.ordering.GetEnumerator();
        }

        public int IndexOf(OrderBy item)
        {
            return this.ordering.IndexOf(item);
        }

        public void Insert(int index, OrderBy item)
        {
            this.ValidateProperty(item);
            this.ordering.Insert(index, item);
        }

        public bool Remove(OrderBy item)
        {
            return this.ordering.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.ordering.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void ValidateProperty(OrderBy item)
        {
            PropertyInfo property = Properties.SingleOrDefault(x => string.Equals(x.Name, item.PropertyName, StringComparison.Ordinal));

            if (property is null)
            {
                throw new PropertyNotFoundException(item.PropertyName, typeof(T).Name);
            }

            if (property.PropertyType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IComparable<>)))
            {
                return;
            }

            if (typeof(IComparable).IsAssignableFrom(property.PropertyType))
            {
                return;
            }

            throw new PropertyNotComparableException(item.PropertyName);
        }
    }
}
