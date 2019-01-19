namespace Scaffold.Domain.Entities
{
    public class Item
    {
        private Bucket bucket;

        public int Id { get; set; }

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

                this.bucket?.RemoveItem(this);
                this.bucket = value;
                this.bucket?.AddItem(this);
            }
        }
    }
}
