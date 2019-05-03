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

        public bool Ascending { get; set; }

        public bool Descending
        {
            get => !this.Ascending;

            set => this.Ascending = !value;
        }
    }
}
