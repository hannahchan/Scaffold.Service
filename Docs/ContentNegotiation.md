# Content Negotiation #

Scaffold.WebApi accepts and returns content of the following media types.

- `application/json`
- `application/xml`

The default media type for both requests and responses is `application/json`.

To get a response from the Web API in a different media type, add the `Accept` header to the request with the value of the media type you would like the response in. For example `Accept: application/xml`.
