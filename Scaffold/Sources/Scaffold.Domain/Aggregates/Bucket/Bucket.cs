namespace Scaffold.Domain.Aggregates.Bucket;

using System.Collections.Generic;

public class Bucket
{
    private readonly List<Item> items;

    private int size = 5;

    public Bucket()
        : this(default)
    {
    }

    public Bucket(int id)
    {
        this.items = new List<Item>();
        this.Id = id;
    }

    public int Id { get; private set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int Size
    {
        get => this.size;

        set
        {
            if (value < 0)
            {
                throw new InvalidSizeException(
                    "Size cannot be set to a negative number.");
            }

            if (value < this.items.Count)
            {
                throw new InvalidSizeException(
                    "Size cannot be set less than the number of items already in the bucket.");
            }

            this.size = value;
        }
    }

    public IReadOnlyList<Item> Items => this.items;

    public void AddItem(Item item)
    {
        if (this.items.Contains(item))
        {
            return;
        }

        if (this.IsFull())
        {
            throw new BucketFullException($"Bucket '{this.Id}' is full. Cannot add Item to Bucket.");
        }

        this.items.Add(item);
    }

    public bool IsFull()
    {
        return this.items.Count >= this.size;
    }

    public void RemoveItem(Item item)
    {
        this.items.Remove(item);
    }
}
