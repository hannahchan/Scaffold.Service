# Docker Support #

Scaffold.WebApi has been created with Docker support in mind so that you can build a container image of your Web API to be deployed into production or run locally without installing the prerequisites .NET Core and PostgreSQL. Docker support is facilitated by the `docker-compose.yml` file included in the root of this project.

## Building a Container Image of the Web API ##

To build a container image of your Web API using the latest base images, simply run;

`docker-compose build --force-rm --pull`

This command produces a production ready image of your ASP.NET Core Web API.

## Running the Web API Locally in Docker ##

Scaffold.WebApi can be run locally in Docker without installing the prerequisites .NET Core and PostgreSQL. To run the Web API in Docker locally and in the background, run;

`docker-compose up -d`

If a container image of your Web API doesn't exist, this command will also build it. Once up and running you can access your Web API by going to `http://localhost`.

To stop your Web API running in Docker, run;

`docker-compose down`

This will stop your Web API and the PostgreSQL containers that were running in the background. This command will also leave behind a Docker volume belonging to PostgreSQL which contains any data that was persisted to the database. To remove this volume, run;

`docker-compose down --volume`

## Running the PostgreSQL Database Only ##

When developing your Web API, you may only want to run the PostgreSQL database locally in Docker and not the Web API. To do this use the command;

`docker-compose up -d postgres`

This is ideal if you don't want to install PostgreSQL onto your computer or need to manage different versions for different projects. In this scenario you would run your Web API natively on your machine.
