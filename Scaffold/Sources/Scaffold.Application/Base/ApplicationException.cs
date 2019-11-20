namespace Scaffold.Application.Base
{
    using System;
    using System.Runtime.Serialization;

    public abstract class ApplicationException : Exception
    {
        private readonly string? title;

        protected ApplicationException(string? message)
            : base(message)
        {
        }

        protected ApplicationException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected ApplicationException(string? title, string? message)
            : base(message)
        {
            this.title = title;
        }

        protected ApplicationException(string? title, string? message, Exception? innerException)
            : base(message, innerException)
        {
            this.title = title;
        }

        protected ApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.title = info.GetString(nameof(this.Title));
        }

        public virtual string Detail => this.Message;

        public virtual string Title => this.title ?? "Application Exception";

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(this.Title), this.Title);
            base.GetObjectData(info, context);
        }
    }
}