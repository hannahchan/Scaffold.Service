namespace Scaffold.Application.Exceptions
{
    using System;

    public abstract class IntegrationException : ApplicationException
    {
        protected IntegrationException()
        {
        }

        protected IntegrationException(string message)
            : base(message)
        {
        }

        protected IntegrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public virtual int Status { get; set; }
    }
}
