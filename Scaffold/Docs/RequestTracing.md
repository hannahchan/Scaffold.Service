# Request Tracing #

Request tracing is the practice of tagging interactions with an identifer to see how an interaction flows within a service, or across multiple services in a system. Request tracing is most useful when used with logging and is often used in microservices environments to help troubleshoot issues. HTTP interactions are typically tagged by including a predefined HTTP header with the value of an identifer. Identifiers are often referred to as *Correlation IDs* or *Request IDs*.

## The Request Identifier Scheme ##

Scaffold makes a distinction between the terms *Correlation ID* and *Request ID*.

### Correlation ID ###

A Correlation ID is an external identifer that is passed in with a request to correlate that request. It is not guaranteed to be unique and is expected to be pass to downstream services if calls to those services are required to service the request. The goal of the Correlation ID is to provide a mechanism to *group* requests.

### Request ID ###

A Request ID is an internally generated identifer for a request that uniquely identifies that request. It coexists with the Correlation ID. If no Correlation ID was passed in with the request, the Request ID *becomes* the Correlation ID and is expected to be pass to downstream services if calls to those services are required to service the request. The goal of the Request ID is to provide a mechanism to identify a request.

### Request Tracing Header ###

The default request tracing header for Scaffold is `Correlation-Id` and has been defined as a constant in a file named [CustomHeaderNames.cs](../Sources/Scaffold.WebApi/Constants/CustomHeaderNames.cs). Changing this value will change the name of the request tracing header that the Web API will accept and send out.

## Incoming Request Behavior ##

When a Correlation ID is present in an incoming HTTP request, the [RequestTracingMiddleware](../Sources/Scaffold.WebApi/Middleware/RequestTracingMiddleware.cs) copies the Correlation ID to an instance of [RequestTracingService](../Sources/Scaffold.WebApi/Services/RequestTracingService.cs). This service is intended to be dependency injected into your application to wherever you need it and is scoped to the lifetime of the HTTP request. If there is no Correlation ID present in the incoming HTTP request, the internally generated Request ID is copied to the *RequestTracingService* as the Correlation ID.

The *RequestTracingMiddleware* also adds the value of both the Correlation ID and Request ID to the headers of the HTTP response for the incoming HTTP request.

## Outbound Request Behavior ##

Included with Scaffold is a [delegating handler](https://docs.microsoft.com/aspnet/core/fundamentals/http-requests#outgoing-request-middleware) class called [RequestTaggingHttpMessageHandler](../Sources/Scaffold.WebApi/HttpMessageHandlers/RequestTaggingHttpMessageHandler.cs) that can be used with instances of *HttpClient*. This handler tags all outbound requests for a given instance of *HttpClient* with a predefined set of headers including one for the Correlation ID. Configure your HttpClients to use the *RequestTaggingHttpMessageHandler* in the [Startup](../Sources/Scaffold.WebApi/Startup.cs) class.
