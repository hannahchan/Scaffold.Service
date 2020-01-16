namespace Scaffold.Application.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using Scaffold.Application.Base;

    [Serializable]
    public class PropertyNotComparableException : OrderingException
    {
        public PropertyNotComparableException(string propertyName)
            : base($"\"{propertyName}\" is not a comparable property.")
        {
        }

        protected PropertyNotComparableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
