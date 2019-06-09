namespace Scaffold.Application.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public abstract class ApplicationException : Exception
    {
        protected ApplicationException(string message)
            : base(message)
        {
        }

        protected ApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ApplicationException(string title, string message)
            : base(message) => this.Title = title;

        protected ApplicationException(string title, string message, Exception innerException)
            : base(message, innerException) => this.Title = title;

        protected ApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context) => this.Title = info.GetString(nameof(this.Title));

        public virtual string Detail => this.Message;

        public virtual string Title { get; private set; } = "Application Exception";

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(this.Title), this.Title);
            base.GetObjectData(info, context);
        }
    }
}
