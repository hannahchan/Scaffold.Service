# Logging

There is mostly nothing special about logging in Scaffold. Scaffold uses the [built-in logging framework](https://docs.microsoft.com/aspnet/core/fundamentals/logging) that comes with ASP.NET Core.

## HTTP Logging

Scaffold comes with custom high-performance request logging [middleware](../Sources/Scaffold.WebApi/Middleware/RequestLoggingMiddleware.cs) and a [message handler](../Sources/Scaffold.WebApi/HttpMessageHandlers/RequestLoggingHttpMessageHandler.cs) to log inbound and outbound HTTP requests using the [_LoggerMessage_](https://docs.microsoft.com/aspnet/core/fundamentals/logging/loggermessage) pattern. In order to avoid logging sensitive information and performance penalties, these loggers are limited and do not log request / response headers and bodies.

For more feature rich logging, please consider using one of the official ASP.NET loggers instead.

- [HTTP Logging](https://docs.microsoft.com/aspnet/core/fundamentals/http-logging)
- [W3C Logger](https://docs.microsoft.com/aspnet/core/fundamentals/w3c-logger)

## Event Logging

Included in the application layer of Scaffold is an [_EventLogger_](../Sources/Scaffold.Application/Components/Audit/EventLogger.cs) that logs all messages that pass through the [in-process event bus](./Architecture.md). When publishing messages to the in-process event bus, developers should be conscious that sensitive information in the message might be logged.

## Separation of Concerns

Developers should be mindful on why they are logging. Generally reasons for logging fall into two categories;

1. Application Health (Diagnostics)
2. Domain Requirements (Auditing)

Each category will have its own storage requirements.

### Application Health (Diagnostics)

In this category of logging, you are logging _Application Events_ to determine if your application is working correctly. You are going to be using these logs to look for and troubleshoot problems with your application. Examples of these types of logs include;

- Application startup and shutdown
- Database connections and queries
- HTTP requests and responses
- Triggered and processed events

In this category, the more verbose logs are generally more valuable in the short to medium term after which you will most likely never look at them again. You only need enough information long enough to fix your application. Generally you can discard these logs after a relatively short period of time.

In well architected applications that scale, you will probably not look at more than 99% of this category of logs. It’s physically impossible for a human and there are better tools in the application metrics and distributed tracing space to help you troubleshoot. That 99% of logs you don’t look at has a cost. Sampling is recommended here to help you balance cost to verboseness.

### Domain Requirements (Auditing)

In this category of logging, you are logging _Domain Events_ to determine what happened in your domain and you are also most likely required to keep these logs for the whole lifetime of the application or domain. Examples of logs that fall into this category include;

- Access control
- Approval workflows
- Transactions

It is possible and many developer do (often naively) use the same logging backend/store to record _Domain Events_ as you would your _Application Events_. If your domain requires this kind of logging, it is recommended that you build this into your domain model and persist these _Domain Events_ into the same data store that contains your other domain objects.
