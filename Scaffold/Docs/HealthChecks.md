# Health Checks

Scaffold uses a basic implementation of the [Health Check Middleware](https://learn.microsoft.com/aspnet/core/host-and-deploy/health-checks) included in ASP.NET Core. The health status of Scaffold can be check at `/health` and returns a HTTP 200 OK response when the service is healthy. This health check is intended to be used by monitoring services.

## Health Checks for Monitoring Services

Health checks that check instances of an application are usually intended for container orchestrators and load balancers to make decisions on whether or not to put instances of an application into service or to take them out. This is the type of health check that has been implemented in Scaffold.

## Health Check Port

Health checks usually only need to be exposed internally. By default, the health check endpoint in Scaffold is exposed on the same port as the rest of the application. To specify a different port, set the environment variable `ASPNETCORE_HEALTHCHECKPORT` with the new port and update `ASPNETCORE_URLS` to match.

For example;

    ASPNETCORE_HEALTHCHECKPORT=8081
    ASPNETCORE_URLS=http://+:80;http://+:8081

## Entity Framework Core DbContext Check

As part of the health check, Scaffold checks whether or not it can establish a connection to the database. This health check does not check the Entity Framework Migration state of the database. It is also not an indicator of whether or not the database is up or down.
