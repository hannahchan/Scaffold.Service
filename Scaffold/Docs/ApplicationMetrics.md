# Application Metrics

Scaffold currently uses [prometheus-net](https://github.com/prometheus-net/prometheus-net) to instrument and expose application metrics. These metrics are by default exposed on the `/metrics` endpoint of the Web API and is intended to be _scraped_ by [Prometheus](https://prometheus.io/). You can change this endpoint by modifying the [`Startup.cs`](../Sources/Scaffold.WebApi/Startup.cs) file of the Web API. You'll need to further instrument your application if you require further metrics than those provided by the default implementation.

A future version of Scaffold will switch away from [prometheus-net](https://github.com/prometheus-net/prometheus-net) and instead will use the [OpenTelemetry](https://opentelemetry.io) observability framework for application metrics.

## Metrics Port

When you build a container image of Scaffold and run it, the metrics endpoint is exposed on port `8081` while the rest of the application remains on port `80`. Port `80` is intended to be the public port while port `8081` is the private port intended for monitoring services. You can change the ports used in the Scaffold container image by modifying the [Dockerfile](../Sources/Scaffold.WebApi/Dockerfile) used to build it and the [docker-compose.yml](../docker-compose.yml) used to run it.

When Scaffold is run locally, the metrics endpoint is still exposed on port `8081` while the rest of the application is on port `5000`. You can change this by modifying [launchSettings.json](../Sources/Scaffold.WebApi/Properties/launchSettings.json).

## Example Queries

If you're new to Prometheus and its query language PromQL, it can be difficult to figure out how to get the metrics you want out of Prometheus. We recommend reading the documentation on how to write PromQL here;

<https://prometheus.io/docs/prometheus/latest/querying/basics/>.

We've included some example below to help you get started.

### Latency

Average HTTP Request Duration in Milliseconds (Over the Last 5 Minutes)

    rate(http_request_duration_seconds_sum[5m]) /
    rate(http_request_duration_seconds_count[5m]) * 1000

Request Duration of the 85th Percentile in Milliseconds (Over the Last 5 Minutes)

    histogram_quantile(0.85, sum(rate(http_request_duration_seconds_bucket[5m])) by (le)) * 1000

### Traffic

Number of HTTP Requests (Over the Last 5 Minutes)

    increase(http_requests_received_total[5m])

Average Requests Per Minute (Over the Last 5 Minutes)

    rate(http_requests_received_total[5m]) * 60

Instant Requests Per Minute (Using Last 2 Data Points in Last 5 Minutes)

    irate(http_requests_received_total[5m]) * 60

Current Number of Concurrent Requests

    http_requests_in_progress

Maximum Number of Concurrent Requests (In the Last 24 Hours)

    max_over_time(http_requests_in_progress[24h])

### Errors

Number of Client Errors (4xx) (Over the Last 5 Minutes)

    increase(http_requests_received_total{code=~"4.*"}[5m])

Average Client Errors (4xx) Per Minute (Over the Last 5 Minutes)

    rate(http_requests_received_total{code=~"4.*"}[5m]) * 60

Percentage of All Client Errors (4xx) to All Requests (Over the Last 5 Minutes)

    sum(increase(http_requests_received_total{code=~"4.*"}[5m])) /
    sum(increase(http_requests_received_total{code=~".+"}[5m])) * 100

Number of Server Errors (5xx) (Over the Last 5 Minutes)

    increase(http_requests_received_total{code=~"5.*"}[5m])

Average Server Errors (5xx) Per Minute (Over the Last 5 Minutes)

    rate(http_requests_received_total{code=~"5.*"}[5m]) * 60

Percentage of All Server Errors (5xx) to All Requests (Over the Last 5 Minutes)

    sum(increase(http_requests_received_total{code=~"5.*"}[5m])) /
    sum(increase(http_requests_received_total{code=~".+"}[5m])) * 100
