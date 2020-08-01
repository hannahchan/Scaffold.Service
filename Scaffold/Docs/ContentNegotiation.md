# Content Negotiation

Scaffold accepts and returns content in the following media types.

- `application/json`
- `application/xml`

The default media type for both requests and responses is `application/json`.

When sending a request to the Web API with content, add the `Content-Type` header with the value of the media type that best describes the format of your content. For example `Content-Type: application/xml`.

To get a response from the Web API in a different media type, add the `Accept` header to the request with the value of the media type you would like the response in. For example `Accept: application/xml`.
