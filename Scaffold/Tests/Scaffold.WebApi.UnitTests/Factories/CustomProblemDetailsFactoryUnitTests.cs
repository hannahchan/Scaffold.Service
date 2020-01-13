namespace Scaffold.WebApi.UnitTests.Factories
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using OpenTracing;
    using OpenTracing.Mock;
    using Scaffold.WebApi.Factories;
    using Xunit;

    public class CustomProblemDetailsFactoryUnitTests
    {
        public class Constructor
        {
            [Fact]
            public void When_InstantiatingFactory_Expect_Instantiated()
            {
                // Arrange
                IOptions<ApiBehaviorOptions> options = Options.Create(new ApiBehaviorOptions());

                // Act
                Exception result = Record.Exception(() => new CustomProblemDetailsFactory(options));

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void When_InstantiatingFactoryWithNullOptions_Expect_ArgumentNullException()
            {
                // Act
                Exception result = Record.Exception(() => new CustomProblemDetailsFactory(null!));

                // Assert
                Assert.NotNull(result);
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("options", argumentNullException.ParamName);
            }

            [Fact]
            public void When_InstantiatingFactoryWithNullApiBehaviorOptions_Expect_ArgumentNullException()
            {
                // Arrange
                IOptions<ApiBehaviorOptions> options = Options.Create<ApiBehaviorOptions>(null!);

                // Act
                Exception result = Record.Exception(() => new CustomProblemDetailsFactory(options));

                // Assert
                Assert.NotNull(result);
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("options", argumentNullException.ParamName);
            }
        }

        public class CreateProblemDetails
        {
            [Fact]
            public void When_CreatingProblemDetails_Expect_ProblemDetails()
            {
                // Arrange
                IOptions<ApiBehaviorOptions> options = Options.Create(new ApiBehaviorOptions());
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                // Act
                ProblemDetails result = factory.CreateProblemDetails(new DefaultHttpContext());

                // Assert
                Assert.Equal(500, result.Status);
                Assert.Null(result.Title);
                Assert.Null(result.Type);
                Assert.Null(result.Detail);
                Assert.Null(result.Instance);
                Assert.Empty(result.Extensions);
            }

            [Fact]
            public void When_CreatingProblemDetailsWithNullHttpContext_Expect_ArgumentNullException()
            {
                // Arrange
                IOptions<ApiBehaviorOptions> options = Options.Create(new ApiBehaviorOptions());
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                // Act
                Exception result = Record.Exception(() => factory.CreateProblemDetails(null!));

                // Assert
                Assert.NotNull(result);
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("httpContext", argumentNullException.ParamName);
            }

            [Fact]
            public void When_CreatingProblemDetailsWithValues_Expect_ProblemDetailsWithValues()
            {
                // Arrange
                IOptions<ApiBehaviorOptions> options = Options.Create(new ApiBehaviorOptions());
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                int status = new Random().Next(int.MaxValue);
                string title = Guid.NewGuid().ToString();
                string type = Guid.NewGuid().ToString();
                string detail = Guid.NewGuid().ToString();
                string instance = Guid.NewGuid().ToString();

                // Act
                ProblemDetails result = factory.CreateProblemDetails(new DefaultHttpContext(), status, title, type, detail, instance);

                // Assert
                Assert.Equal(status, result.Status);
                Assert.Equal(title, result.Title);
                Assert.Equal(type, result.Type);
                Assert.Equal(detail, result.Detail);
                Assert.Equal(instance, result.Instance);
                Assert.Empty(result.Extensions);
            }

            [Fact]
            public void When_CreatingProblemDetailsWithClientErrorMapping_Expect_ProblemDetailsWithClientErrorMapped()
            {
                // Arrange
                ApiBehaviorOptions apiBehaviorOptions = new ApiBehaviorOptions();

                int status = new Random().Next(int.MaxValue);

                apiBehaviorOptions.ClientErrorMapping[status] = new ClientErrorData
                {
                    Title = Guid.NewGuid().ToString(),
                    Link = Guid.NewGuid().ToString(),
                };

                IOptions<ApiBehaviorOptions> options = Options.Create(apiBehaviorOptions);
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                // Act
                ProblemDetails result = factory.CreateProblemDetails(new DefaultHttpContext(), status);

                // Assert
                Assert.Equal(status, result.Status);
                Assert.Equal(apiBehaviorOptions.ClientErrorMapping[status].Title, result.Title);
                Assert.Equal(apiBehaviorOptions.ClientErrorMapping[status].Link, result.Type);
            }

            [Fact]
            public void When_CreatingProblemDetailsWithClientErrorMapping_Expect_ProblemDetailsWithClientErrorNotMapped()
            {
                // Arrange
                ApiBehaviorOptions apiBehaviorOptions = new ApiBehaviorOptions();

                int status = new Random().Next(int.MaxValue);

                apiBehaviorOptions.ClientErrorMapping[status] = new ClientErrorData
                {
                    Title = Guid.NewGuid().ToString(),
                    Link = Guid.NewGuid().ToString(),
                };

                IOptions<ApiBehaviorOptions> options = Options.Create(apiBehaviorOptions);
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                string title = Guid.NewGuid().ToString();
                string type = Guid.NewGuid().ToString();

                // Act
                ProblemDetails result = factory.CreateProblemDetails(new DefaultHttpContext(), status, title, type);

                // Assert
                Assert.NotEqual(apiBehaviorOptions.ClientErrorMapping[status].Title, title);
                Assert.NotEqual(apiBehaviorOptions.ClientErrorMapping[status].Link, type);
                Assert.Equal(status, result.Status);
                Assert.Equal(title, result.Title);
                Assert.Equal(type, result.Type);
            }

            [Fact]
            public void When_CreatingProblemDetailsWithActiveSpan_Expect_ProblemDetailsWithTraceId()
            {
                // Arrange
                ServiceCollection services = new ServiceCollection();
                services.AddScoped<ITracer, MockTracer>();

                IServiceProvider serviceProvider = services.BuildServiceProvider();
                ITracer tracer = serviceProvider.GetRequiredService<ITracer>();

                IOptions<ApiBehaviorOptions> options = Options.Create(new ApiBehaviorOptions());
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                ProblemDetails result;

                // Act
                using (tracer.BuildSpan("Unit Test").StartActive())
                {
                    result = factory.CreateProblemDetails(new DefaultHttpContext { RequestServices = serviceProvider });
                }

                // Assert
                MockTracer mockTracer = Assert.IsType<MockTracer>(tracer);
                MockSpan mockSpan = Assert.Single(mockTracer.FinishedSpans());
                Assert.Equal(mockSpan.Context.TraceId, result.Extensions["traceId"]);
            }
        }

        public class CreateValidationProblemDetails
        {
            [Fact]
            public void When_CreatingValidationProblemDetails_Expect_ValidationProblemDetails()
            {
                // Arrange
                IOptions<ApiBehaviorOptions> options = Options.Create(new ApiBehaviorOptions());
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                // Act
                ValidationProblemDetails result =
                    factory.CreateValidationProblemDetails(new DefaultHttpContext(), new ModelStateDictionary());

                // Assert
                Assert.Equal(400, result.Status);
                Assert.Equal("One or more validation errors occurred.", result.Title);
                Assert.Null(result.Type);
                Assert.Null(result.Detail);
                Assert.Null(result.Instance);
                Assert.Empty(result.Extensions);
            }

            [Fact]
            public void When_CreatingValidationProblemDetailsWithNullHttpContext_Expect_ArgumentNullException()
            {
                // Arrange
                IOptions<ApiBehaviorOptions> options = Options.Create(new ApiBehaviorOptions());
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                // Act
                Exception result = Record.Exception(() => factory.CreateValidationProblemDetails(null!, new ModelStateDictionary()));

                // Assert
                Assert.NotNull(result);
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("httpContext", argumentNullException.ParamName);
            }

            [Fact]
            public void When_CreatingValidationProblemDetailsWithNullModelState_Expect_ArgumentNullException()
            {
                // Arrange
                IOptions<ApiBehaviorOptions> options = Options.Create(new ApiBehaviorOptions());
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                // Act
                Exception result = Record.Exception(() => factory.CreateValidationProblemDetails(new DefaultHttpContext(), null!));

                // Assert
                Assert.NotNull(result);
                ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("modelStateDictionary", argumentNullException.ParamName);
            }

            [Fact]
            public void When_CreatingValidationProblemDetailsWithValues_Expect_ValidationProblemDetailsWithValues()
            {
                // Arrange
                IOptions<ApiBehaviorOptions> options = Options.Create(new ApiBehaviorOptions());
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                int status = new Random().Next(int.MaxValue);
                string title = Guid.NewGuid().ToString();
                string type = Guid.NewGuid().ToString();
                string detail = Guid.NewGuid().ToString();
                string instance = Guid.NewGuid().ToString();

                // Act
                ValidationProblemDetails result =
                    factory.CreateValidationProblemDetails(new DefaultHttpContext(), new ModelStateDictionary(), status, title, type, detail, instance);

                // Assert
                Assert.Equal(status, result.Status);
                Assert.Equal(title, result.Title);
                Assert.Equal(type, result.Type);
                Assert.Equal(detail, result.Detail);
                Assert.Equal(instance, result.Instance);
                Assert.Empty(result.Extensions);
            }

            [Fact]
            public void When_CreatingValidationProblemDetailsWithClientErrorMapping_Expect_ValidationProblemDetailsWithClientErrorMapped()
            {
                // Arrange
                ApiBehaviorOptions apiBehaviorOptions = new ApiBehaviorOptions();

                int status = new Random().Next(int.MaxValue);

                apiBehaviorOptions.ClientErrorMapping[status] = new ClientErrorData
                {
                    Title = Guid.NewGuid().ToString(),
                    Link = Guid.NewGuid().ToString(),
                };

                IOptions<ApiBehaviorOptions> options = Options.Create(apiBehaviorOptions);
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                // Act
                ValidationProblemDetails result =
                    factory.CreateValidationProblemDetails(new DefaultHttpContext(), new ModelStateDictionary(), status);

                // Assert
                Assert.Equal(status, result.Status);
                Assert.Equal("One or more validation errors occurred.", result.Title);
                Assert.Equal(apiBehaviorOptions.ClientErrorMapping[status].Link, result.Type);
            }

            [Fact]
            public void When_CreatingValidationProblemDetailsWithClientErrorMapping_Expect_ValidationProblemDetailsWithClientErrorNotMapped()
            {
                // Arrange
                ApiBehaviorOptions apiBehaviorOptions = new ApiBehaviorOptions();

                int status = new Random().Next(int.MaxValue);

                apiBehaviorOptions.ClientErrorMapping[status] = new ClientErrorData
                {
                    Title = Guid.NewGuid().ToString(),
                    Link = Guid.NewGuid().ToString(),
                };

                IOptions<ApiBehaviorOptions> options = Options.Create(apiBehaviorOptions);
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                string title = Guid.NewGuid().ToString();
                string type = Guid.NewGuid().ToString();

                // Act
                ValidationProblemDetails result =
                    factory.CreateValidationProblemDetails(new DefaultHttpContext(), new ModelStateDictionary(), status, title, type);

                // Assert
                Assert.NotEqual(apiBehaviorOptions.ClientErrorMapping[status].Title, title);
                Assert.NotEqual(apiBehaviorOptions.ClientErrorMapping[status].Link, type);
                Assert.Equal(status, result.Status);
                Assert.Equal(title, result.Title);
                Assert.Equal(type, result.Type);
            }

            [Fact]
            public void When_CreatingValidationProblemDetailsWithActiveSpan_Expect_ValidationProblemDetailsWithTraceId()
            {
                // Arrange
                ServiceCollection services = new ServiceCollection();
                services.AddScoped<ITracer, MockTracer>();

                IServiceProvider serviceProvider = services.BuildServiceProvider();
                ITracer tracer = serviceProvider.GetRequiredService<ITracer>();

                IOptions<ApiBehaviorOptions> options = Options.Create(new ApiBehaviorOptions());
                ProblemDetailsFactory factory = new CustomProblemDetailsFactory(options);

                ValidationProblemDetails result;

                // Act
                using (tracer.BuildSpan("Unit Test").StartActive())
                {
                    result = factory.CreateValidationProblemDetails(
                        new DefaultHttpContext { RequestServices = serviceProvider },
                        new ModelStateDictionary());
                }

                // Assert
                MockTracer mockTracer = Assert.IsType<MockTracer>(tracer);
                MockSpan mockSpan = Assert.Single(mockTracer.FinishedSpans());
                Assert.Equal(mockSpan.Context.TraceId, result.Extensions["traceId"]);
            }
        }
    }
}
