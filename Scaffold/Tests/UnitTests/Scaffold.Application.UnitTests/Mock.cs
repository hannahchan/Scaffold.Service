namespace Scaffold.Application.UnitTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public static class Mock
    {
        public class Publisher : IPublisher
        {
            public List<Event> PublishedEvents { get; } = new List<Event>();

            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                this.PublishedEvents.Add(new Event(notification, cancellationToken));
                return Task.CompletedTask;
            }

            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
                where TNotification : INotification
            {
                this.PublishedEvents.Add(new Event(notification, cancellationToken));
                return Task.CompletedTask;
            }

            public record Event(object Notification, CancellationToken CancellationToken);
        }
    }
}
