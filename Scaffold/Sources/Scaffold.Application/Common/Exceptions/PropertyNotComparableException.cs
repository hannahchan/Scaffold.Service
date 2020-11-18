namespace Scaffold.Application.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PropertyNotComparableException : SortOrderException
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
