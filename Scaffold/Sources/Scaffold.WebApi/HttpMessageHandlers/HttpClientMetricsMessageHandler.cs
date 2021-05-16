namespace Scaffold.WebApi.HttpMessageHandlers
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Prometheus;

    public class HttpClientMetricsMessageHandler : DelegatingHandler
    {
        private static readonly string[] RequestLabelNames = new string[] { "method", "host" };

        private static readonly string[] RequestAndResponseLabelNames = new string[] { "method", "host", "code" };

        private static readonly Gauge InProgress = Metrics.CreateGauge(
            "httpclient_requests_in_progress",
            "Number of requests currently being executed by an HttpClient.",
            new GaugeConfiguration
            {
                LabelNames = RequestLabelNames,
            });

        private static readonly Counter RequestCount = Metrics.CreateCounter(
            "httpclient_requests_received_total",
            "Count of HTTP requests that have been completed by an HttpClient.",
            new CounterConfiguration
            {
                LabelNames = RequestAndResponseLabelNames,
            });

        private static readonly Histogram RequestDuration = Metrics.CreateHistogram(
            "httpclient_request_duration_seconds",
            "Duration histogram of HTTP requests performed by an HttpClient.",
            new HistogramConfiguration
            {
                // 1 ms to 32K ms buckets
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 16),
                LabelNames = RequestAndResponseLabelNames,
            });

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri is null)
            {
                throw new InvalidOperationException("Missing RequestUri while processing request.");
            }

            HttpResponseMessage response;
            TimeSpan elapsedTime;

            using (InProgress.WithLabels(request.Method.Method, request.RequestUri.Host).TrackInProgress())
            {
                ValueStopwatch stopwatch = ValueStopwatch.StartNew();

                response = await base.SendAsync(request, cancellationToken);
                elapsedTime = stopwatch.GetElapsedTime();
            }

            string statusCode = ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture);

            RequestCount
                .WithLabels(request.Method.Method, request.RequestUri.Host, statusCode)
                .Inc();

            RequestDuration
                .WithLabels(request.Method.Method, request.RequestUri.Host, statusCode)
                .Observe(elapsedTime.TotalSeconds);

            return response;
        }

        // Based on: https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/Extensions/ValueStopwatch/ValueStopwatch.cs
        private struct ValueStopwatch
        {
            private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

            private readonly long startTimestamp;

            private ValueStopwatch(long startTimestamp)
            {
                this.startTimestamp = startTimestamp;
            }

            public static ValueStopwatch StartNew()
            {
                return new ValueStopwatch(Stopwatch.GetTimestamp());
            }

            public TimeSpan GetElapsedTime()
            {
                long endTimestamp = Stopwatch.GetTimestamp();
                long timestampDelta = endTimestamp - this.startTimestamp;
                long ticks = (long)(TimestampToTicks * timestampDelta);
                return new TimeSpan(ticks);
            }
        }
    }
}
