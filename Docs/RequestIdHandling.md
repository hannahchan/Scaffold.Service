# Request ID Handling #

Request IDs, sometimes known as Correlation IDs, are unique identifiers used to track interactions that cross multiple service boundaries in a system. They are most useful when used with logging to troubleshoot issues in a microservices environment.

HTTP requests to Scaffold.WebApi can be tagged with a request ID by including the header `Request-Id` in the request.

## Changing the Default Request ID Header ##

The default request ID header for Scaffold.WebApi has been defined as a constant in a file named [Headers.cs](../Sources/Scaffold.WebApi/Constants/Headers.cs) in the [Scaffold.WebApi](../Sources/Scaffold.WebApi) project. Changing this value will change the name of the request ID header the that Web API will accept.

## The Request ID Scheme ##

The default behavior of Scaffold.WebApi is to only log interactions when a request ID has been provided. It does not automatically generate one when the request ID header is absent. This behavior can be changed by modifying [RequestIdMiddleware](../Sources/Scaffold.WebApi/Middleware/RequestIdMiddleware.cs) in the [Scaffold.WebApi](../Sources/Scaffold.WebApi) project.

## Using the Request ID ##

When a request ID header is present in a HTTP request, the [RequestIdMiddleware](../Sources/Scaffold.WebApi/Middleware/RequestIdMiddleware.cs) copies the value of the request ID header to an instance of [RequestIdService](../Sources/Scaffold.WebApi/Services/RequestIdService.cs). This service is intended to be dependency injected into your application to wherever you need it and is scoped to the lifetime of the HTTP request. You can then pull the request ID out of this service to be used in your application.
