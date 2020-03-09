# Application Metrics #

Scaffold comes with basic instrumentation of application metrics. These metrics are by default exposed on the `/metrics` endpoint of the Web API and is intended to be *scraped* by [Prometheus](https://prometheus.io/). You can change this endpoint by modifying the [`Startup.cs`](../Sources/Scaffold.WebApi/Startup.cs) file of the Web API. You'll need to further instrument your application if you require further metrics than those provided by the default implementation. For more information head over to;

https://github.com/prometheus-net/prometheus-net

## Metrics Port ##

When you build a container image of Scaffold and run it, the metrics endpoint is exposed on port `8081` while the rest of the application remains on port `80`. Port `80` is intended to be the public port while port `8081` is the private port intended for monitoring services. You can change the ports used in the Scaffold container image by modifying the [Dockerfile](../Sources/Scaffold.WebApi/Dockerfile) used to build it and the [docker-compose.yml](../docker-compose.yml) used to run it.

When Scaffold is run locally, the metrics endpoint is exposed on the same port as the rest of the application. For example on `http://localhost:5000/metrics`. Change this by modifying [launchSettings.json](../Sources/Scaffold.WebApi/Properties/launchSettings.json).

## Local Development with Prometheus ##

If you need to experiment with how Prometheus scraps your application and play with what ends up in Prometheus during local development, you'll need to install and run Prometheus locally. Check out the [Getting started](https://prometheus.io/docs/prometheus/latest/getting_started/) guide on the Prometheus website on how to do this.

Included in the root of this project is an example [`prometheus.yml`](../prometheus.yml) configuration file to help get you started quickly.

```yaml
# An example Prometheus config for local development

global:
  scrape_interval: 15s
scrape_configs:
  -
    job_name: scaffold
    static_configs:
      -
        targets:
          - "localhost:5000"
```

Once Prometheus has been started, you can navigate to the expression browser on `http://localhost:9090/graph`.
