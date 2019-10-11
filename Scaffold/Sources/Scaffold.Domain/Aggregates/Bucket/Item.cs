namespace Scaffold.Domain.Aggregates.Bucket
{
    public class Item
    {
        public Item()
            : this(default)
        {
        }

        public Item(int id)
        {
            this.Id = id;
        }

        public int Id { get; private set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
