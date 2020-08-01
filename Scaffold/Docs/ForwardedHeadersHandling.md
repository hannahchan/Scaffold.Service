# Forwarded Headers Handling

Requests sent to Web APIs are often relayed through network appliances such as proxies or load balancers before reaching the Web APIs. When this happens, information about the original request is often transposed into the headers of the request that is eventually received by the Web API. Common headers used to do this are:

- `X-Forwarded-For`
- `X-Forwarded-Proto`
- `X-Forwarded-Host`

When Scaffold receives a request with any of these headers present, it sets certain values in the [HttpContext](https://docs.microsoft.com/dotnet/api/system.web.httpcontext) object to the values contained in the headers so that you can consume them in your application.

- `HttpContext.Connection.RemoteIpAddress` is set by `X-Forwarded-For`.
- `HttpContext.Request.Scheme` is set by `X-Forwarded-Proto`.
- `HttpContext.Request.Host` is set by `X-Forwarded-Host`.

This is all handled by the Forwarded Headers middleware built into ASP.NET Core. For more information on how to configure this middleware, please checkout;

https://docs.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer
