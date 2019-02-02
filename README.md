# Scaffold.WebApi - Not a real Web API #

Scaffold.WebApi is an opinionated template of a Web API built using ASP.NET Core. It is intended to be used as an example for, or as a bootstrap to, the development of other Web APIs and aims to address some common concerns when building microservices using Web APIs in ASP.NET Core.

## How to Use ##

To bootstrap your next WebApi project with Scaffold.WebApi.

1. Clone this repository into your project directory.
2. Delete the `.git` folder.
3. Find and replace the words `Scaffold` and `scaffold` with the name of your project.
4. Update `README.md` so that it makes sense.

## Example App ##

Scaffold.WebApi includes an example application which is intended to be replaced with your actual application when using this template. This example application has been included to help demonstrate features in this template.

The example application is a simple CRUD application where you can create **Buckets** and put **Items** in them. The Buckets have a *size* which represents the number of Items you can put in them. To *Create*, *Read*, *Update* and *Delete* Buckets or Items in the application, simply send a HTTP *POST*, *GET*, *PATCH* or *DELETE* request to the Web API.

Explore the Web API by going to `/swagger` in a web browser.

## Developing and Running the Web API ##

For the best experience when developing with and running Scaffold.WebApi, it is recommended to have Docker installed. Docker is not required however parts of the documentation and support features has been written or built assuming that you have Docker installed.

Scaffold.WebApi requires a PostgreSQL database before it can run and one can be stood up quickly using Docker. Alternatively you could install PostgreSQL on to your local machine. For more information about on how to run the Web API with Docker, checkout our [Docker Support](Docs/Docker.md) page in the [Docs](Docs) directory.

[Visual Studio Code (vscode) Support](Docs/VisualStudioCode.md) in this project also assumes you have Docker installed.
