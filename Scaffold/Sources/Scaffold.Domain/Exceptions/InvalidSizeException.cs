namespace Scaffold.Domain.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class InvalidSizeException : DomainException
    {
        public InvalidSizeException(string message)
            : base("Invalid Size", message)
        {
        }

        protected InvalidSizeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
