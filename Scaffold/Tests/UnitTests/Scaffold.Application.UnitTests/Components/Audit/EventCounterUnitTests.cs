namespace Scaffold.Application.UnitTests.Components.Audit;

using System;
using System.Threading.Tasks;
using MediatR;
using Prometheus;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Audit;
using Xunit;

public class EventCounterUnitTests
{
    public class Handle
    {
        [Fact]
        public async Task When_HandlingAuditableEvent_Expect_NoExceptions()
        {
            // Arrange
            EventCounter<AuditableEvent> eventCounter = new EventCounter<AuditableEvent>();
            AuditableEvent auditableEvent = new AuditableEvent(default, default, default, "MyType", "MyDescription");

            // Act
            Exception exception = await Record.ExceptionAsync(() => eventCounter.Handle(auditableEvent, default));

            // Assert
            Assert.Null(exception);

            Counter counter = EventCounterAccessor.GetCounter();
            Assert.Equal("application_events_total", counter.Name);
            Assert.Equal("Total number of events that have been published to the in-process event bus", counter.Help);
            Assert.Equal(1, counter.Labels("MyType").Value);
        }

        [Fact]
        public async Task When_HandlingGenericEvent_Expect_NoExceptions()
        {
            // Arrange
            EventCounter<GenericEvent> eventCounter = new EventCounter<GenericEvent>();
            GenericEvent genericEvent = new GenericEvent();

            // Act
            Exception exception = await Record.ExceptionAsync(() => eventCounter.Handle(genericEvent, default));

            // Assert
            Assert.Null(exception);

            Counter counter = EventCounterAccessor.GetCounter();
            Assert.Equal("application_events_total", counter.Name);
            Assert.Equal("Total number of events that have been published to the in-process event bus", counter.Help);
            Assert.Equal(1, counter.Labels(nameof(GenericEvent)).Value);
        }
    }

    private record AuditableEvent(DateTime Timestamp, string TraceId, Type Source, string Type, string Description) : IAuditableEvent, INotification;

    private record GenericEvent() : INotification;

    private class EventCounterAccessor : EventCounterBase
    {
        public static Counter GetCounter() => EventCounter;
    }
}
