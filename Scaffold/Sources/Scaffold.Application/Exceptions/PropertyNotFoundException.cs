namespace Scaffold.Application.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using Scaffold.Application.Base;

    [Serializable]
    public class PropertyNotFoundException : OrderingException
    {
        public PropertyNotFoundException(string propertyName, string type)
            : base($"\"{propertyName}\" is not a property of \"{type}\".")
        {
        }

        protected PropertyNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
