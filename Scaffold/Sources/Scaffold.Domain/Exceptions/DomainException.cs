namespace Scaffold.Domain.Exceptions
{
    using System;

    public abstract class DomainException : Exception
    {
        protected DomainException()
            : base()
        {
        }

        protected DomainException(string message)
            : base(message)
        {
        }

        protected DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public virtual string Detail { get; set; }

        public virtual string Title { get; set; }
    }
}
