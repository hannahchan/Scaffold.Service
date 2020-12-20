namespace Scaffold.WebApi.UnitTests.Extensions
{
    using System;
    using Microsoft.Extensions.Hosting;
    using Scaffold.WebApi.Extensions;
    using Xunit;

    public class HostExtensionsUnitTests
    {
        [Fact]
        public void When_EnsuringCreatedDatabaseWithNullHost_Expect_ArgumentNullException()
        {
            // Arrange
            IHost host = null;

            // Act
            Exception exception = Record.Exception(() => host.EnsureCreatedDatabase());

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("host", argumentNullException.ParamName);
        }

        [Fact]
        public void When_MigratingDatabaseWithNullHost_Expect_ArgumentNullException()
        {
            // Arrange
            IHost host = null;

            // Act
            Exception exception = Record.Exception(() => host.MigrateDatabase());

            // Assert
            ArgumentNullException argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("host", argumentNullException.ParamName);
        }
    }
}
