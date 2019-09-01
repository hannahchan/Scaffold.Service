namespace Scaffold.WebApi.UnitTests.Extensions
{
    using Scaffold.WebApi.Extensions;
    using Xunit;

    public class StringExtensionUnitTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("string", "string")]
        [InlineData("String", "string")]
        [InlineData("STRING", "string")]
        [InlineData("stringExtension", "stringExtension")]
        [InlineData("StringExtension", "stringExtension")]
        [InlineData("STRINGExtension", "stringExtension")]
        [InlineData("string Extension", "string Extension")]
        [InlineData("String Extension", "string Extension")]
        [InlineData("STRING Extension", "string Extension")]
        public void When_InvokingToCamelCase_Expect_CamelCaseString(string @string, string expectedString)
        {
            // Assert
            Assert.Equal(expectedString, @string.ToCamelCase());
        }
    }
}
