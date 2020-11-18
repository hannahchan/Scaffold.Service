namespace Scaffold.Application.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PropertyNotFoundException : SortOrderException
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
