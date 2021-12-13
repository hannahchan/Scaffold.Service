# Scaffold.Service

Scaffold.Service is an opinionated template of a service built using ASP.NET Core. It is intended to be used as an example for, or as a bootstrap to, the development of other services and aims to address some common concerns when building microservices with ASP.NET Core. Scaffold is an example of a [Service Template](https://www.thoughtworks.com/radar/techniques/tailored-service-templates).

## How to Use

To bootstrap your next service with Scaffold.Service.

1. Install the template package from [NuGet.org](https://www.nuget.org/packages/Scaffold.Service) onto your computer by running;

   ```
   dotnet new --install Scaffold.Service
   ```

2. Create your new service by running;

   ```
   dotnet new scaffold.service --output <SOLUTION_NAME>
   ```

3. Update `README.md` and all other documentation in your service so that it makes sense.

Alternatively just browse the [template directory](Scaffold) to copy and paste the parts of the service that you actually need.

## Features

Scaffold.Service comes pre-configured with or aims to make developing the following features in your next service a little bit more easier.

- [API Documentation](./Scaffold/Docs/ApiDocumentation.md)
- [Application Metrics](./Scaffold/Docs/ApplicationMetrics.md)
- [Content Negotiation](./Scaffold/Docs/ContentNegotiation.md)
- [Distributed Tracing](./Scaffold/Docs/DistributedTracing.md)
- [Logging](./Scaffold/Docs/Logging.md)
- [Health Checks](./Scaffold/Docs/HealthChecks.md)
- [Problem Details (RFC 7807) Error Handling](./Scaffold/Docs/ProblemDetails.md)

### Developer Experience

Other stuff you inherit when you bootstrap with Scaffold.Service.

- [Architecture](./Scaffold/Docs/Architecture.md)
- [Code Analysis and Formatting](./Scaffold/Docs/CodeAnalysisAndFormatting.md)
- [Deterministic Builds](./Scaffold/Docs/DeterministicBuilds.md)
- [Docker Support](./Scaffold/Docs/Docker.md)
- [Entity Framework Support](./Scaffold/Docs/EntityFramework.md)
- [Nullable Reference Types](./Scaffold/Docs/NullableReferenceTypes.md)
- [Test Structure](./Scaffold/Docs/TestStructure.md)

### Not Included

Stuff you might want to add to your service but not included with Scaffold.Service.

- [API Versioning](https://github.com/Microsoft/aspnet-api-versioning)
- [Authentication](https://docs.microsoft.com/aspnet/core/security/authentication) and [Authorization](https://docs.microsoft.com/aspnet/core/security/authorization)
- [Console Log Formatting](https://docs.microsoft.com/dotnet/core/extensions/console-log-formatter)
- [Cross-Origin Resource Sharing (CORS)](https://docs.microsoft.com/aspnet/core/security/cors)
- [Forwarded Headers Handling](https://docs.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer)
- [Header Propagation](https://docs.microsoft.com/aspnet/core/fundamentals/http-requests#header-propagation-middleware)
- [Secrets Management](https://docs.microsoft.com/aspnet/core/security/app-secrets)

## Example App

Scaffold.Service includes an example application which is intended to be replaced with your actual application when using this template. This example application has been included to help demonstrate features in this template.

The example application is a simple CRUD application where you can create **Buckets** and put **Items** in them. The Buckets have a _size_ which represents the number of Items you can put in them. To _Create_, _Read_, _Update_ and _Delete_ Buckets or Items in the application, simply send HTTP _POST_, _GET_, _PUT_ or _DELETE_ requests to the service.

Explore the service by going to `/swagger` in a web browser.

## Create Your Own

You are also invited to fork this repository to create your own template. Organizations may want to do this to pre-configure the template to work in their environments and incorporate their own sensible defaults.

For more information about custom templates, please checkout [_Custom templates for dotnet new_](https://docs.microsoft.com/dotnet/core/tools/custom-templates).
