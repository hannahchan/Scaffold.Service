# Scaffold

Scaffold is an opinionated template of a Web API built using ASP.NET Core. It is intended to be used as an example for, or as a bootstrap to, the development of other Web APIs and aims to address some common concerns when building microservices using Web APIs in ASP.NET Core. Scaffold is an example of a [Tailored Service Template](https://www.thoughtworks.com/radar/techniques/tailored-service-templates).

## Getting Started

Before you start developing Scaffold, please read our [Getting Started](Docs/GettingStarted.md) guide. It outlines what you need to install on your local machine as well as how to build, test and run the project.

## Features

Scaffold aims to make developing the following features in your next Web API or microservice a little bit more easier.

- [API Documentation](Docs/ApiDocumentation.md)
- [Application Metrics](Docs/ApplicationMetrics.md)
- [Distributed Tracing](Docs/DistributedTracing.md)
- [Content Negotiation](Docs/ContentNegotiation.md)
- [Health Checks](Docs/HealthChecks.md)
- [Problem Details (RFC 7807) Error Handling](Docs/ProblemDetails.md)

### Other Features

Other stuff you inherit when you bootstrap with Scaffold.

- [Architecture](Docs/Architecture.md)
- [Docker Support](Docs/Docker.md)
- [Entity Framework Support](Docs/EntityFramework.md)
- [Roslyn Analyzers](Docs/RoslynAnalyzers.md)

### Not Included

Stuff you might want to add to your Web API / microservice but not included with Scaffold.

- [API Versioning](https://github.com/Microsoft/aspnet-api-versioning)
- [Authentication](https://docs.microsoft.com/aspnet/core/security/authentication) and [Authorization](https://docs.microsoft.com/aspnet/core/security/authorization)
- [Console Log Formatting](https://docs.microsoft.com/dotnet/core/extensions/console-log-formatter)
- [Cross-Origin Resource Sharing (CORS)](https://docs.microsoft.com/aspnet/core/security/cors)
- [Forwarded Headers Handling](https://docs.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer)
- [Header Propagation](https://docs.microsoft.com/aspnet/core/fundamentals/http-requests#header-propagation-middleware)
- [Secrets Management](https://docs.microsoft.com/aspnet/core/security/app-secrets)

## Example App

Scaffold includes an example application which is intended to be replaced with your actual application when using this template. This example application has been included to help demonstrate features in this template.

The example application is a simple CRUD application where you can create **Buckets** and put **Items** in them. The Buckets have a _size_ which represents the number of Items you can put in them. To _Create_, _Read_, _Update_ and _Delete_ Buckets or Items in the application, simply send HTTP _POST_, _GET_, _PUT_ or _DELETE_ requests to the Web API.

Explore the Web API by going to `/swagger` in a web browser.
