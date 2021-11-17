# Scaffold.WebApi

Scaffold is an opinionated template of a Web API built using ASP.NET Core. It is intended to be used as an example for, or as a bootstrap to, the development of other Web APIs or services and aims to address some common concerns when building microservices with ASP.NET Core. Scaffold is an example of a [Tailored Service Template](https://www.thoughtworks.com/radar/techniques/tailored-service-templates).

## How to Use

To bootstrap your next WebApi project with Scaffold.WebApi.

1. Clone this repository and install the template on your computer by running;

   ```
   dotnet new --install Scaffold
   ```

2. Create your new project by running;

   ```
   dotnet new scaffold.webapi --output <PROJECT_NAME>
   ```

3. Update `README.md` in your project so that it makes sense.

Alternatively just browse the [template directory](Scaffold) and copy and paste the parts of the project that you actually need.

## Features

Scaffold.WebApi aims to make developing the following features in your next Web API or microservice a little bit more easier.

- [API Documentation](./Scaffold/Docs/ApiDocumentation.md)
- [Application Metrics](./Scaffold/Docs/ApplicationMetrics.md)
- [Content Negotiation](./Scaffold/Docs/ContentNegotiation.md)
- [Distributed Tracing](./Scaffold/Docs/DistributedTracing.md)
- [Logging](./Scaffold/Docs/Logging.md)
- [Health Checks](./Scaffold/Docs/HealthChecks.md)
- [Problem Details (RFC 7807) Error Handling](./Scaffold/Docs/ProblemDetails.md)

### Other Features

Other stuff you inherit when you bootstrap with Scaffold.WebApi.

- [Architecture](./Scaffold/Docs/Architecture.md)
- [Code Analysis and Formatting](./Scaffold/Docs/CodeAnalysisAndFormatting.md)
- [Deterministic Builds](./Scaffold/Docs/DeterministicBuilds.md)
- [Docker Support](./Scaffold/Docs/Docker.md)
- [Entity Framework Support](./Scaffold/Docs/EntityFramework.md)
- [Nullable Reference Types](./Scaffold/Docs/NullableReferenceTypes.md)
- [Test Structure](./Scaffold/Docs/TestStructure.md)

### Not Included

Stuff you might want to add to your Web API / microservice but not included with Scaffold.WebApi.

- [API Versioning](https://github.com/Microsoft/aspnet-api-versioning)
- [Authentication](https://docs.microsoft.com/aspnet/core/security/authentication) and [Authorization](https://docs.microsoft.com/aspnet/core/security/authorization)
- [Console Log Formatting](https://docs.microsoft.com/dotnet/core/extensions/console-log-formatter)
- [Cross-Origin Resource Sharing (CORS)](https://docs.microsoft.com/aspnet/core/security/cors)
- [Forwarded Headers Handling](https://docs.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer)
- [Header Propagation](https://docs.microsoft.com/aspnet/core/fundamentals/http-requests#header-propagation-middleware)
- [Secrets Management](https://docs.microsoft.com/aspnet/core/security/app-secrets)

## Example App

Scaffold.WebApi includes an example application which is intended to be replaced with your actual application when using this template. This example application has been included to help demonstrate features in this template.

The example application is a simple CRUD application where you can create **Buckets** and put **Items** in them. The Buckets have a _size_ which represents the number of Items you can put in them. To _Create_, _Read_, _Update_ and _Delete_ Buckets or Items in the application, simply send HTTP _POST_, _GET_, _PUT_ or _DELETE_ requests to the Web API.

Explore the Web API by going to `/swagger` in a web browser.
