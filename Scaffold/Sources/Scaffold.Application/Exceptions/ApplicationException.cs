namespace Scaffold.Application.Exceptions
{
    using System;

    public abstract class ApplicationException : Exception
    {
        protected ApplicationException()
        {
        }

        protected ApplicationException(string message)
            : base(message)
        {
        }

        protected ApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public virtual string Detail { get; set; }

        public virtual string Title { get; set; }
    }
}
