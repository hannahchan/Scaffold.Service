# Scaffold

Scaffold is an opinionated template of a service built using ASP.NET Core. It is intended to be used as an example for, or as a bootstrap to, the development of other services and aims to address some common concerns when building microservices with ASP.NET Core. Scaffold is an example of a [Service Template](https://www.thoughtworks.com/radar/techniques/tailored-service-templates).

## Getting Started

Before you start developing Scaffold, please read our [Getting Started](./Docs/GettingStarted.md) guide. It outlines what you need to install on your local machine as well as how to build, test and run the service.

## Features

Scaffold comes pre-configured with or aims to make developing the following features in your next service a little bit more easier.

- [API Documentation](./Docs/ApiDocumentation.md)
- [Application Metrics](./Docs/ApplicationMetrics.md)
- [Content Negotiation](./Docs/ContentNegotiation.md)
- [Distributed Tracing](./Docs/DistributedTracing.md)
- [Error Handling](./Docs/ErrorHandling.md)
- [Logging](./Docs/Logging.md)
- [Health Checks](./Docs/HealthChecks.md)

### Developer / Team Experience

Other stuff you inherit when you bootstrap with Scaffold.

- [Architecture](./Docs/Architecture.md)
- [Code Analysis and Formatting](./Docs/CodeAnalysisAndFormatting.md)
- [Deterministic Builds](./Docs/DeterministicBuilds.md)
- [Development Container](./Docs/DevelopmentContainer.md)
- [Docker Support](./Docs/Docker.md)
- [Entity Framework Support](./Docs/EntityFramework.md)
- [Nullable Reference Types](./Docs/NullableReferenceTypes.md)
- [Software Bill of Materials](./Docs/SoftwareBillOfMaterials.md)
- [Test Structure](./Docs/TestStructure.md)

### Not Included

Stuff you might want to add to your service but not included with Scaffold.

- [API Versioning](https://github.com/Microsoft/aspnet-api-versioning)
- [Authentication](https://docs.microsoft.com/aspnet/core/security/authentication) and [Authorization](https://docs.microsoft.com/aspnet/core/security/authorization)
- [Caching](https://docs.microsoft.com/dotnet/core/extensions/caching) (See also [Overview of caching in ASP.NET Core](https://docs.microsoft.com/aspnet/core/performance/caching/overview))
- [Console Log Formatting](https://docs.microsoft.com/dotnet/core/extensions/console-log-formatter)
- [Cross-Origin Resource Sharing (CORS)](https://docs.microsoft.com/aspnet/core/security/cors)
- [Forwarded Headers Handling](https://docs.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer)
- [Header Propagation](https://docs.microsoft.com/aspnet/core/fundamentals/http-requests#header-propagation-middleware)
- [Response Compressions](https://docs.microsoft.com/aspnet/core/performance/response-compression)
- [Secrets Management](https://docs.microsoft.com/aspnet/core/security/app-secrets)

## Example App

Scaffold includes an example application which is intended to be replaced with your actual application when using this template. This example application has been included to help demonstrate features in this template.

The example application is a simple CRUD application where you can create **Buckets** and put **Items** in them. The Buckets have a _size_ which represents the number of Items you can put in them. To _Create_, _Read_, _Update_ and _Delete_ Buckets or Items in the application, simply send HTTP _POST_, _GET_, _PUT_ or _DELETE_ requests to the service.

Explore the service by going to `/swagger` in a web browser.

## Acknowledgements

Scaffold has been inspired by and built from the prior experience and work of many others. In particular, Scaffold heavily relies on the open-source community and those who support that community. Please consider contributing back to their open-source projects in whatever way you can.
