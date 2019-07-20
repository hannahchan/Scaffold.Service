namespace Scaffold.Domain.Entities
{
    using System.Linq;
    using Scaffold.Domain.Exceptions;

    public class Item
    {
        private Bucket bucket;

        public Item()
            : this(default)
        {
        }

        public Item(int id) => this.Id = id;

        public int Id { get; private set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Bucket Bucket
        {
            get => this.bucket;

            set
            {
                if (this.bucket == value)
                {
                    return;
                }

                if (value != null && !value.Items.Contains(this) && value.IsFull())
                {
                    throw new BucketFullException($"Bucket '{value.Id}' is full. Cannot add Item to Bucket.");
                }

                this.bucket?.RemoveItem(this);
                this.bucket = value;
                this.bucket?.AddItem(this);
            }
        }
    }
}
