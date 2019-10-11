namespace Scaffold.Domain.Base
{
    using System;
    using System.Runtime.Serialization;

    public abstract class DomainException : Exception
    {
        private readonly string? title;

        protected DomainException(string? message)
            : base(message)
        {
        }

        protected DomainException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected DomainException(string? title, string? message)
            : base(message)
        {
            this.title = title;
        }

        protected DomainException(string? title, string? message, Exception? innerException)
            : base(message, innerException)
        {
            this.title = title;
        }

        protected DomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.title = info.GetString(nameof(this.Title));
        }

        public virtual string Detail => this.Message;

        public virtual string Title => this.title ?? "Domain Exception";

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(this.Title), this.Title);
            base.GetObjectData(info, context);
        }
    }
}
