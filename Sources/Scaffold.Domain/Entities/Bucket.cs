namespace Scaffold.Domain.Entities
{
    using System;
    using System.Collections.Generic;

    public class Bucket
    {
        private readonly List<Item> items;

        public Bucket() => this.items = new List<Item>();

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IReadOnlyCollection<Item> Items => this.items.AsReadOnly();

        public void AddItem(Item item)
        {
            if (this.items.Contains(item))
            {
                return;
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.items.Add(item);
            item.Bucket = this;
        }

        public void RemoveItem(Item item)
        {
            if (!this.items.Contains(item))
            {
                return;
            }

            this.items.Remove(item);
            item.Bucket = null;
        }
    }
}
