namespace Scaffold.Application.Exceptions
{
    public class PropertyNotFoundException : OrderingException
    {
        public PropertyNotFoundException(string propertyName, string type)
            : base($"\"{propertyName}\" is not a property of \"{type}\".")
        {
        }

        public override string Detail => this.Message;

        public override string Title => "Property Not Found";
    }
}
