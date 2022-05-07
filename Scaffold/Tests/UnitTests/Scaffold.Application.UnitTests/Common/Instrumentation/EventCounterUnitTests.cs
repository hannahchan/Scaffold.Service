namespace Scaffold.Application.UnitTests.Common.Instrumentation;

using System;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using MediatR;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Messaging;
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

            Counter<long> counter = EventCounterAccessor.GetCounter();
            Assert.Equal("application.events.count", counter.Name);
            Assert.Equal("events", counter.Unit);
            Assert.Equal("measures the number of events that have been published to the in-process event bus", counter.Description);
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

            Counter<long> counter = EventCounterAccessor.GetCounter();
            Assert.Equal("application.events.count", counter.Name);
            Assert.Equal("events", counter.Unit);
            Assert.Equal("measures the number of events that have been published to the in-process event bus", counter.Description);
        }
    }

    private record AuditableEvent(DateTime Timestamp, string TraceId, Type Source, string Type, string Description) : IAuditableEvent, INotification;

    private record GenericEvent() : INotification;

    private class EventCounterAccessor : EventCounterBase
    {
        public static Counter<long> GetCounter() => EventCounter;
    }
}
