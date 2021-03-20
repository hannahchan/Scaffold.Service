# Getting Started

If you are intending to help develop and contribute to Scaffold, this guide is meant to help you setup your development environment.

## Preparing for Local Development

The local development experience is what most developers will be familiar with. This is where you clone and work off a copy of a repository on your local machine using the tools that you have installed. To get started with local development, you will need to have the following installed on your local machine;

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/engine/install)
- [Docker Compose](https://docs.docker.com/compose/install)
- [Visual Studio Code](https://code.visualstudio.com) (Recommended)

## Building and Testing the Project

Once you have cloned the repository but before you start working on it, it is always a good idea to make sure that you are able to successfully build the project and run the tests.

From the root of the project, to build the project run;

    dotnet build

Likewise to run the tests, run;

    dotnet test

### Creating a Release

To help different developers build and test the project consistently on different machines, a [Cake script](../build.cake) has been included at the root of this project. To run the script, run the command;

    dotnet cake

You may need to run `dotnet tool restore` first if you don't have the dotnet tool `Cake.Tool` installed. By default, the Cake script will build, test and publish the project and copy all related artifacts into the `Artifacts` directory. This directory is where you will find your release ready build output and test coverage reports.

By default, the Cake script does not produce any container images as output. To produce release ready container images, use the following command instead;

    dotnet cake --Target=Containerize

For more information about Cake, please visit <https://cakebuild.net>.

## Running and Debugging the Project

Scaffold is dependent on a PostgreSQL database. Before you can run this project, you will need to stand-up a local PostgreSQL database. You can do this quickly using the provided [docker-compose.yml](../docker-compose.yml) script with the command;

    docker-compose up -d postgres

The docker-compose.yml also includes other service which you can spin up should you need them for development. For more information, please take a look at the [Docker Support](./Docker.md) documentation.

Before the Scaffold can run, the required schema for Scaffold also needs to be created in the database. This is automatically done for you when the environment variable `ASPNETCORE_ENVIRONMENT` is set to `Development`. For more information, please take a look at the [Entity Framework Support](./EntityFramework.md) documentation.

To run Scaffold, simply navigate to the [Scaffold.WebApi](../Sources/Scaffold.WebApi) project and use the command;

    dotnet run

Once running you can attach a debugger from your preferred Integrated Development Environment (IDE).

### The F5 Experience

Although you can work on Scaffold using any Integrated Development Environment (IDE), the development experience has been optimized for Visual Studio Code (vscode). Included in Scaffold is a launch configuration for vscode which uses the docker-compose.yml script to spin up the required services when you start a debug session by pressing `F5`. The services spin down when the debug session ends.

## Developing inside a Container

Scaffold includes configuration which you can use with the [Visual Studio Code Remote - Containers](https://code.visualstudio.com/docs/remote/containers) extension to spin up a development container. This is ideal for those who want to;

- Use a sandboxed development environment.
- Get started faster with a consistent development environment.

Only the following need to be installed on your local machine for this development experience to work.

- [Docker](https://docs.docker.com/engine/install)
- [Docker Compose](https://docs.docker.com/compose/install)
- [Visual Studio Code](https://code.visualstudio.com)

All other tooling should already be installed inside the development container. You are also welcomed to install your own.
