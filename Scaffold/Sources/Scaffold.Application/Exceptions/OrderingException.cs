namespace Scaffold.Application.Exceptions
{
    using System;

    public abstract class OrderingException : ApplicationException
    {
        protected OrderingException()
        {
        }

        protected OrderingException(string message)
            : base(message)
        {
        }

        protected OrderingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
