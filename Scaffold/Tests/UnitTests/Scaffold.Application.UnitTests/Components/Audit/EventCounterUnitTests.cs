namespace Scaffold.Application.UnitTests.Components.Audit;

using System;
using System.Threading.Tasks;
using MediatR;
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
        }
    }

    private record AuditableEvent(DateTime Timestamp, string TraceId, Type Source, string Type, string Description) : IAuditableEvent, INotification;

    private record GenericEvent() : INotification;
}
