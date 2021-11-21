namespace Scaffold.Application.UnitTests.Components.Bucket;

using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Scaffold.Application.Common.Messaging;
using Scaffold.Application.Components.Audit;
using Xunit;

public class AuditLoggerUnitTests
{
    public class Handle
    {
        [Fact]
        public async Task When_HandlingAuditableEvent_Expect_Logged()
        {
            // Arrange
            Mock.Logger<AuditLogger<AuditableEvent>> logger = new Mock.Logger<AuditLogger<AuditableEvent>>();
            AuditLogger<AuditableEvent> auditLogger = new AuditLogger<AuditableEvent>(logger);

            AuditableEvent auditableEvent = new AuditableEvent(default, default, default, "MyType", "MyDescription");

            // Act
            await auditLogger.Handle(auditableEvent, default);

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
            Mock.Logger<AuditLogger<GenericEvent>> logger = new Mock.Logger<AuditLogger<GenericEvent>>();
            AuditLogger<GenericEvent> auditLogger = new AuditLogger<GenericEvent>(logger);

            GenericEvent genericEvent = new GenericEvent();

            // Act
            await auditLogger.Handle(genericEvent, default);

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

    private record AuditableEvent(DateTime Timestamp, string TraceId, string Source, string Type, string Description) : IAuditableEvent, INotification;

    private record GenericEvent() : INotification;
}
