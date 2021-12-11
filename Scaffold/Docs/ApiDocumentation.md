# API Documentation

In a microservices environment, other developers (or stakeholders) may be wondering what your service does and how to use it. Scaffold helps improves the developer experience in this aspect by including API documentation in the service. Scaffold uses a package called [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) to help generate and expose documentation about the service in the [OpenAPI Specification (OAS)](https://www.openapis.org/) format. When running the service, you can view this documentation by going to the `/swagger` endpoint. For example `http://localhost:5000/swagger`.

## Code Generation

When designed and configured correctly, your API documentation can be used as an input to tools such as the [OpenAPI Generator](https://openapi-generator.tech) or [Swagger Codegen](https://swagger.io/tools/swagger-codegen/) to generate client code for your service in any supported language.

For example with the OpenAPI Generator, you can generate a TypeScript client for your service with the following command;

    openapi-generator generate -i http://localhost:5000/swagger/v1/swagger.json -g typescript-fetch

You can then package and distribute your client as a library to be consumed by other developers. This can go a long way to improving the developer experience for consumers of your service.
