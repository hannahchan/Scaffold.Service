namespace Scaffold.Application.Models
{
    public class OrderBy
    {
        public OrderBy(string propertyName, bool ascending = true)
        {
            this.PropertyName = propertyName;
            this.Ascending = ascending;
        }

        public string PropertyName { get; }

        public bool Ascending { get; }

        public bool Descending { get => !this.Ascending; }
    }
}
