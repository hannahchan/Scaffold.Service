namespace Scaffold.WebApi.UnitTests.HttpMessageHandlers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using OpenTracing;
    using OpenTracing.Mock;
    using Scaffold.WebApi.HttpMessageHandlers;
    using Xunit;

    public class OpenTracingSpanTaggingHttpMessageHandlerUnitTests
    {
        [Theory]
        [InlineData(199, true)]
        [InlineData(200, false)]
        [InlineData(299, false)]
        [InlineData(300, true)]
        public async Task When_SendingAsyncRespondsWithStatusCode_Expect_SetTag(int statusCode, bool expectedError)
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<ITracer, MockTracer>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ITracer tracer = serviceProvider.GetRequiredService<ITracer>();

            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext { RequestServices = serviceProvider },
            };

            OpenTracingSpanTaggingHttpMessageHandler handler = new OpenTracingSpanTaggingHttpMessageHandler(httpContextAccessor)
            {
                InnerHandler = new MockResponseReturningInnerHandler(statusCode),
            };

            // Act
            using (tracer.BuildSpan("Unit Test").StartActive())
            using (HttpClient client = new HttpClient(handler))
            {
                await client.GetAsync(new Uri("http://localhost"));
            }

            // Assert
            MockTracer mockTracer = Assert.IsType<MockTracer>(tracer);
            MockSpan mockSpan = Assert.Single(mockTracer.FinishedSpans());

            if (mockSpan.Tags.ContainsKey("error"))
            {
                Assert.Equal(expectedError, mockSpan.Tags["error"]);
            }

            Assert.Equal(expectedError, mockSpan.Tags.ContainsKey("error"));
        }

        [Fact]
        public async Task When_SendingAsyncRespondsWithStatusCodeWithNullActiveSpan_Expect_NoSpans()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<ITracer, MockTracer>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ITracer tracer = serviceProvider.GetRequiredService<ITracer>();

            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext { RequestServices = serviceProvider },
            };

            OpenTracingSpanTaggingHttpMessageHandler handler = new OpenTracingSpanTaggingHttpMessageHandler(httpContextAccessor)
            {
                InnerHandler = new MockResponseReturningInnerHandler(500),
            };

            Exception result;

            // Act
            using (HttpClient client = new HttpClient(handler))
            {
                result = await Record.ExceptionAsync(() => client.GetAsync(new Uri("http://localhost")));
            }

            // Assert
            MockTracer mockTracer = Assert.IsType<MockTracer>(tracer);
            Assert.Empty(mockTracer.FinishedSpans());
            Assert.Null(result);
        }

        [Fact]
        public async Task When_SendingAsyncRespondsWithException_Expect_SetTagError()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<ITracer, MockTracer>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ITracer tracer = serviceProvider.GetRequiredService<ITracer>();

            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext { RequestServices = serviceProvider },
            };

            Exception exception = new Exception();

            OpenTracingSpanTaggingHttpMessageHandler handler = new OpenTracingSpanTaggingHttpMessageHandler(httpContextAccessor)
            {
                InnerHandler = new MockExceptionThrowingInnerHandler(exception),
            };

            Exception result;

            // Act
            using (tracer.BuildSpan("Unit Test").StartActive())
            using (HttpClient client = new HttpClient(handler))
            {
                result = await Record.ExceptionAsync(() => client.GetAsync(new Uri("http://localhost")));
            }

            // Assert
            MockTracer mockTracer = Assert.IsType<MockTracer>(tracer);
            MockSpan mockSpan = Assert.Single(mockTracer.FinishedSpans());
            Assert.True(mockSpan.Tags.ContainsKey("error"));
            Assert.True(Assert.IsType<bool>(mockSpan.Tags["error"]));

            Assert.NotNull(result);
            Assert.Equal(exception, result);
        }

        [Fact]
        public async Task When_SendingAsyncRespondsWithExceptionWithNullActiveSpan_Expect_NoSpans()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<ITracer, MockTracer>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ITracer tracer = serviceProvider.GetRequiredService<ITracer>();

            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext { RequestServices = serviceProvider },
            };

            Exception exception = new Exception();

            OpenTracingSpanTaggingHttpMessageHandler handler = new OpenTracingSpanTaggingHttpMessageHandler(httpContextAccessor)
            {
                InnerHandler = new MockExceptionThrowingInnerHandler(exception),
            };

            Exception result;

            // Act
            using (HttpClient client = new HttpClient(handler))
            {
                result = await Record.ExceptionAsync(() => client.GetAsync(new Uri("http://localhost")));
            }

            // Assert
            MockTracer mockTracer = Assert.IsType<MockTracer>(tracer);
            Assert.Empty(mockTracer.FinishedSpans());
            Assert.NotNull(result);
            Assert.Equal(exception, result);
        }

        [Fact]
        public async Task When_SendingAsyncWithNullHttpContext_Expect_InvalidOperationException()
        {
            // Arrange
            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = null,
            };

            OpenTracingSpanTaggingHttpMessageHandler handler = new OpenTracingSpanTaggingHttpMessageHandler(httpContextAccessor);

            Exception result;

            // Act
            using (HttpClient client = new HttpClient(handler))
            {
                result = await Record.ExceptionAsync(() => client.GetAsync(new Uri("http://localhost")));
            }

            // Assert
            Assert.NotNull(result);
            InvalidOperationException invalidOperationException = Assert.IsType<InvalidOperationException>(result);
            Assert.Equal("Missing HttpContext while processing request.", invalidOperationException.Message);
        }

        [Fact]
        public async Task When_SendingAsyncWithNoTracerRegisteredInServices_Expect_InvalidOperationException()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext { RequestServices = serviceProvider },
            };

            OpenTracingSpanTaggingHttpMessageHandler handler = new OpenTracingSpanTaggingHttpMessageHandler(httpContextAccessor);

            Exception result;

            // Act
            using (HttpClient client = new HttpClient(handler))
            {
                result = await Record.ExceptionAsync(() => client.GetAsync(new Uri("http://localhost")));
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<InvalidOperationException>(result);
        }

        private class MockResponseReturningInnerHandler : DelegatingHandler
        {
            private readonly int statusCode;

            public MockResponseReturningInnerHandler(int statusCode)
            {
                this.statusCode = statusCode;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage { StatusCode = (HttpStatusCode)this.statusCode });
            }
        }

        private class MockExceptionThrowingInnerHandler : DelegatingHandler
        {
            private readonly Exception exception;

            public MockExceptionThrowingInnerHandler(Exception exception)
            {
                this.exception = exception;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromException<HttpResponseMessage>(this.exception);
            }
        }
    }
}
