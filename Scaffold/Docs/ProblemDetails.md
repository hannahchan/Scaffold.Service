# Problem Details (RFC 7807) Error Handling

Scaffold implements [RFC 7807 - Problem Details for HTTP APIs](https://tools.ietf.org/html/rfc7807) for communicating most errors to consumers of the Web API in a machine readable format. Error responses are returned in the media type formats `application/problem+json` or `application/problem+xml`.

The following is an example error response from the Web API in JSON;

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "Bucket '1' is full. Cannot add Item to Bucket.",
  "traceId": "00-50eecc9647dd4a47bce7f69cfc683d98-13025c0ce39c224e-00"
}
```

And the same response in XML;

```xml
<problem xmlns="urn:ietf:rfc:7807">
  <detail>Bucket '1' is full. Cannot add Item to Bucket.</detail>
  <status>409</status>
  <title>Conflict</title>
  <type>https://tools.ietf.org/html/rfc7231#section-6.5.8</type>
  <traceId>00-50eecc9647dd4a47bce7f69cfc683d98-13025c0ce39c224e-00</traceId>
</problem>
```

Error responses by default include a `traceId` property. This `traceId` property is a [W3C Trace Context](https://www.w3.org/TR/trace-context).

## How it works

Scaffold uses an [exception filter](../Sources/Scaffold.WebApi/Filters/ExceptionFilter.cs) in the ASP.NET Core MVC filter pipeline to catch exceptions and convert them into a _Problem Details_ response. Exceptions that are not caught by the exception filter are not converted in to a _Problem Details_ response and is instead handled in the middleware pipeline.
