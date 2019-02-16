# Problem Details (RFC 7807) Error Handling #

Scaffold.WebApi implements [RFC 7807 - Problem Details for HTTP APIs](https://tools.ietf.org/html/rfc7807) for communicating most errors to consumers of the Web API in a machine readable format. Error responses are returned in the media type formats `application/problem+json` or `application/problem+xml`.

The following is an example error response from the Web API in JSON;

```
{
    "title": "Bucket Full",
    "status": 409,
    "detail": "Bucket '1' is full. Cannot add Item to Bucket."
}
```

And the same response in XML;

```
<problem xmlns="urn:ietf:rfc:7807">
    <detail>Bucket '1' is full. Cannot add Item to Bucket.</detail>
    <status>409</status>
    <title>Bucket Full</title>
</problem>
```

## How it works ##

Scaffold.WebApi uses an [exception filter](../Sources/Scaffold.WebApi/Filters/ExceptionFilter.cs) in the ASP.NET Core MVC filter pipeline to catch exceptions and convert them into a *Problem Details* response. Exceptions that are not caught by the exception filter are not converted in to a *Problem Details* response and is instead handled by [middleware](../Sources/Scaffold.WebApi/Middleware/UnhandledExceptionMiddleware.cs).
