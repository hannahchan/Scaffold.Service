namespace Scaffold.Application.UnitTests.Common.Instrumentation;

using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Scaffold.Application.Common.Instrumentation;
using Scaffold.Application.Common.Messaging;
using Xunit;

public class EventLoggerUnitTests
{
    public class Handle
    {
        [Fact]
        public async Task When_HandlingAuditableEvent_Expect_Logged()
        {
            // Arrange
            Mock.Logger<AuditableEvent> logger = new Mock.Logger<AuditableEvent>();
            EventLogger<AuditableEvent> eventLogger = new EventLogger<AuditableEvent>(logger);

            AuditableEvent auditableEvent = new AuditableEvent(default, default, default, default, "MyType", "MyDescription");

            // Act
            await eventLogger.Handle(auditableEvent, default);

            // Assert
            Assert.Collection(
                logger.LogEntries,
                logEntry =>
                {
                    Assert.Equal(LogLevel.Information, logEntry.LogLevel);
                    Assert.Equal("MyType - MyDescription", logEntry.Message);
                });
        }

        [Fact]
        public async Task When_HandlingGenericEvent_Expect_Logged()
        {
            // Arrange
            Mock.Logger<GenericEvent> logger = new Mock.Logger<GenericEvent>();
            EventLogger<GenericEvent> eventLogger = new EventLogger<GenericEvent>(logger);

            GenericEvent genericEvent = new GenericEvent();

            // Act
            await eventLogger.Handle(genericEvent, default);

            // Assert
            Assert.Collection(
                logger.LogEntries,
                logEntry =>
                {
                    Assert.Equal(LogLevel.Information, logEntry.LogLevel);
                    Assert.Equal($"Received Event {typeof(GenericEvent).FullName}", logEntry.Message);
                });
        }
    }

    private record AuditableEvent(DateTime Timestamp, string TraceId, string SpanId, Type Source, string Type, string Description) : IAuditableEvent, INotification;

    private record GenericEvent() : INotification;
}
