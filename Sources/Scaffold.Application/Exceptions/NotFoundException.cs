namespace Scaffold.Application.Exceptions
{
    using System;

    public abstract class NotFoundException : ApplicationException
    {
        protected NotFoundException()
        {
        }

        protected NotFoundException(string message)
            : base(message)
        {
        }

        protected NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
