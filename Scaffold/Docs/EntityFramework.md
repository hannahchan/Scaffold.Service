# Entity Framework Support #

The example app included with Scaffold uses a PostgreSQL database with [Entity Framework Core](https://docs.microsoft.com/ef). When starting the example app for the first time, you will need to perform a Entity Framework migration to create the schema required by the app in the PostgreSQL database. The Entity Framework migrations are located in the [Scaffold.Repositories.PostgreSQL](../Sources/Adapters/Scaffold.Repositories.PostgreSQL) project.

## Automatic Migrations ##

If you examine [Program.cs](../Sources/Scaffold.WebApi/Program.cs) in the [Scaffold.WebApi](../Sources/Scaffold.WebApi) project, you will notice that there is an [extension method](../Sources/Scaffold.WebApi/Extensions/HostExtension.cs) that does the Entity Framework migration for you when you launch the Web API. This extension method only performs the Entity Framework migration if the environment variable `ASPNETCORE_ENVIRONMENT` has been set to `Development` and has only been included to make the developer's experience a little bit more easier. It is advised to do Entity Framework migrations manually if deploying to production.
