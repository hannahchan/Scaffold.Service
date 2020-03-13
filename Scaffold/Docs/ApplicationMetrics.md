# Application Metrics #

Scaffold comes with basic instrumentation of application metrics. These metrics are by default exposed on the `/metrics` endpoint of the Web API and is intended to be *scraped* by [Prometheus](https://prometheus.io/). You can change this endpoint by modifying the [`Startup.cs`](../Sources/Scaffold.WebApi/Startup.cs) file of the Web API. You'll need to further instrument your application if you require further metrics than those provided by the default implementation. For more information head over to;

https://github.com/prometheus-net/prometheus-net

## Metrics Port ##

When you build a container image of Scaffold and run it, the metrics endpoint is exposed on port `8081` while the rest of the application remains on port `80`. Port `80` is intended to be the public port while port `8081` is the private port intended for monitoring services. You can change the ports used in the Scaffold container image by modifying the [Dockerfile](../Sources/Scaffold.WebApi/Dockerfile) used to build it and the [docker-compose.yml](../docker-compose.yml) used to run it.

When Scaffold is run locally, the metrics endpoint is still exposed on port `8081` while the rest of the application is on port `5000`. You can change this by modifying [launchSettings.json](../Sources/Scaffold.WebApi/Properties/launchSettings.json).
