# Entity Framework Support

Scaffold uses [Entity Framework Core](https://docs.microsoft.com/ef) with a PostgreSQL database. Before the Scaffold can run, the required schema for the app needs to be created in the database. This is done using [Entity Framework Migrations](https://docs.microsoft.com/ef/core/managing-schemas/migrations).

## Entity Framework Core Command Line Tools

The [Entity Framework Core Command Line Tools](https://docs.microsoft.com/ef/core/cli/dotnet) have been included in the [tools manifest](../dotnet-tools.json) of this project. To install you can simple run;

    dotnet tool restore

The command line tools are required to perform design-time tasks such as managing and applying Entity Framework Migrations.

## Managing Migrations

Because the Entity Framework Migrations are located in a different project from the Web API, you may need to specify the paths to the _target project_ and the _startup project_ when running `dotnet ef` commands using the `--project` and `--startup-project` options respectively. The target and startup projects are:

- Target Project - [Scaffold.Repositories](../Sources/Adapters/Scaffold.Repositories)
- Startup Project- [Scaffold.WebApi](../Sources/Scaffold.WebApi)

Scaffold also includes multiple DbContexts. When running `dotnet ef` commands you may also need to specify the DBContext class that you want to operate on using the `--context` option.

For example, to add a migration you can use the following command;

    dotnet ef migrations add "Initial_Create" --project ./Sources/Adapters/Scaffold.Repositories --startup-project ./Sources/Scaffold.WebApi --context BucketContext

For more information about managing migrations, please check out the [official documentation](https://docs.microsoft.com/ef/core/managing-schemas/migrations/managing).

## Applying Migrations

For more detailed and up-to-date information about applying Entity Framework Migrations, please check out the [official documentation](https://docs.microsoft.com/ef/core/managing-schemas/migrations/applying).

### Applying Migrations at Runtime

To make the developer experience more friendly, Scaffold has been configured to automatically apply Entity Framework Migrations on application startup and only does this when the environment variable `ASPNETCORE_ENVIRONMENT` has been set to `Development`. You can see this at work by examining [Program.cs](../Sources/Scaffold.WebApi/Program.cs) in the [Scaffold.WebApi](../Sources/Scaffold.WebApi) project.

Applying migrations in this way is not recommended for production environments.

### Applying Migrations using the Command Line

To manually apply Entity Framework Migrations to the database, you can use the command;

    dotnet ef database update --startup-project ./Sources/Scaffold.WebApi --context BucketContext

By default the connection string used is the one that was specified in the `AddDbContext()` extension method in the startup project. To manually specify the connection string, you can add the `--connection` option followed by your connection string.

Alternatively if you have different `appsettings.json` files set up for each of your different environments with different connections strings, you can [specify the environment](https://docs.microsoft.com/ef/core/cli/dotnet#aspnet-core-environment) instead of using the `--conection` option.

### Applying Migrations using a generated SQL Script

The recommended way to deploy migrations to a production database is by using a generated SQL script. You can generate a SQL script by using the following command;

    dotnet ef migrations script --startup-project ./Sources/Scaffold.WebApi --context BucketContext --output MySqlScript.sql

Applying migrations using a generated SQL script has the advantage of allowing you to inspect what will be run on the database.

The `dotnet ef migrations script` command has several options which may be useful to you for your scenario. Make sure to take a look at them in the [command line reference](https://docs.microsoft.com/ef/core/cli/dotnet#dotnet-ef-migrations-script).
