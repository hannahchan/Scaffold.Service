# Request Tracing #

Request tracing is the practice of tagging interactions with an identifer to see how an interaction flows within a service, or across multiple services in a system. Request tracing is most useful when used with logging and is often used in microservices environments to help troubleshoot issues. HTTP interactions are typically tagged by including a predefined HTTP header with the value of the identifer. Identifiers are often referred to as *Correlation IDs* or *Request IDs*.

## The Request Identifier Scheme ##

Scaffold makes a distinction between the terms *Correlation ID* and *Request ID*.

### Correlation ID ###

A Correlation ID is an external identifer that is passed in with a request to correlate that request. It is not guaranteed to be unique and is expected to be pass to downstream services if calls to those services are required to service the request. The goal of the Correlation ID is to provide a mechanism to *group* requests.

### Request ID ###

A Request ID is an internally generated identifer for a request that uniquely identifies that request. It coexists with the Correlation ID. If no Correlation ID was passed in with the request, the Request ID *becomes* the Correlation ID and is expected to be pass to downstream services if calls to those services are required to service the request. The goal of the Request ID is to provide a mechanism to identify a request.

## Changing the Default Behavior and Header ##

The default request tracing header for Scaffold is `Correlation-Id` and has been defined as a constant in a file named [Headers.cs](../Sources/Scaffold.WebApi/Constants/Headers.cs). Changing this value will change the name of the request tracing header the that Web API will accept.

The default request tracing behavior can be changed by modifying [RequestTracingMiddleware.cs](../Sources/Scaffold.WebApi/Middleware/RequestTracingMiddleware.cs). How request identifiers are logged can be changed by modifying [RequestLoggingMiddleware.cs](../Sources/Scaffold.WebApi/Middleware/RequestLoggingMiddleware.cs).

## The Request Tracing Service ##

When a Correlation ID is present in a HTTP request, the [RequestTracingMiddleware](../Sources/Scaffold.WebApi/Middleware/RequestTracingMiddleware.cs) copies the Correlation ID to an instance of [RequestTracingService](../Sources/Scaffold.WebApi/Services/RequestTracingService.cs). This service is intended to be dependency injected into your application to wherever you need it and is scoped to the lifetime of the HTTP request. You can then pull the Correlation ID out of this service to be used in your application. For example to pass onto downstream services.
