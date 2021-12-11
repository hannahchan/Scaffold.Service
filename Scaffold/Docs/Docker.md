# Docker Support

Scaffold has been created with [Docker](https://www.docker.com/) support in mind so that you can build a container image of the service to be run locally without installing the prerequisites .NET Core and PostgreSQL. Docker support is facilitated by the `docker-compose.yml` file included in the root of this solution.

## Building a Container Image of the Service

To build a container image of the service using the latest base images, simply run;

    docker-compose build --force-rm --pull

## Running the Service Locally in Docker

Scaffold can be run locally in Docker without installing the prerequisites .NET Core and PostgreSQL. To run the service in Docker locally and in the background, run;

    docker-compose up -d

If a container image of the service doesn't exist, this command will also build it. Once up and running you can access the service on `http://localhost`.

To stop the service running in Docker, run;

    docker-compose down

This will stop the service and the PostgreSQL containers that were running in the background. This command will also leave behind a Docker volume belonging to PostgreSQL which contains any data that was persisted to the database. To remove this volume, run;

    docker-compose down --volume

## Running the PostgreSQL Database Only

When developing the service, you may only want to run the PostgreSQL database locally in Docker and not the service. To do this, use the command;

    docker-compose up -d postgres

This is ideal if you don't want to install PostgreSQL onto your computer or need to manage different versions for different projects. In this scenario you would run the service natively on your machine.

To run the service natively on your computer, change directory to the [Scaffold.WebApi](../Sources/Scaffold.WebApi) project under [Sources](../Sources) and run;

    dotnet run

Once up and running you can access the service on `http://localhost:5000`.

## Running other Services Locally

Included in the `docker-compose.yml` file are other services that you can spin up in Docker should you need them for local development. These services and the path to their UIs once spun up are:

- Jaeger - <http://localhost:16686/search>
- Prometheus - <http://localhost:9090/graph>
- Grafana - <http://localhost:3000>

To spin up any combination of these services, use the command;

    docker-compose up -d [service...]

For example to spin up Jaeger, Prometheus an Grafana in one command, use;

    docker-compose up -d jaeger prometheus grafana
