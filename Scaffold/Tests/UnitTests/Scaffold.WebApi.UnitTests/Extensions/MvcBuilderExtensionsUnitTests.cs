namespace Scaffold.WebApi.UnitTests.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Scaffold.WebApi.Extensions;
    using Xunit;

    public class MvcBuilderExtensionsUnitTests
    {
        [Fact]
        public void When_AddingCustomJsonOptionsWithNullMvcBuilder_Expect_ArgumentNullException()
        {
            // Arrange
            IMvcBuilder builder = null;

            // Act
            Exception exception = Record.Exception(() => builder.AddCustomJsonOptions());

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("builder", argumentNullException.ParamName);
        }

        [Fact]
        public void When_AddingCustomXmlFormattersWithNullMvcBuilder_Expect_ArgumentNullException()
        {
            // Arrange
            IMvcBuilder builder = null;

            // Act
            Exception exception = Record.Exception(() => builder.AddCustomXmlFormatters());

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("builder", argumentNullException.ParamName);
        }
    }
}
