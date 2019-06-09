namespace Scaffold.Domain.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public abstract class DomainException : Exception
    {
        protected DomainException(string message)
            : base(message)
        {
        }

        protected DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DomainException(string title, string message)
            : base(message) => this.Title = title;

        protected DomainException(string title, string message, Exception innerException)
            : base(message, innerException) => this.Title = title;

        protected DomainException(SerializationInfo info, StreamingContext context)
            : base(info, context) => this.Title = info.GetString(nameof(this.Title));

        public virtual string Detail => this.Message;

        public virtual string Title { get; private set; } = "Domain Exception";

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(this.Title), this.Title);
            base.GetObjectData(info, context);
        }
    }
}
