namespace Scaffold.Application.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PropertyNotFoundException : OrderingException
    {
        public PropertyNotFoundException(string propertyName, string type)
            : base("Property Not Found", $"\"{propertyName}\" is not a property of \"{type}\".")
        {
        }

        protected PropertyNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
