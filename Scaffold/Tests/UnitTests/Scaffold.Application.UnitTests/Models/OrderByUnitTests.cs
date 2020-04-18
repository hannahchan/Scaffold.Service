namespace Scaffold.Application.UnitTests.Models
{
    using Scaffold.Application.Models;
    using Xunit;

    public class OrderByUnitTests
    {
        [Fact]
        public void When_InstantiatingOrderBy_Expect_AscendingTrueAndDescendingFalse()
        {
            // Arrange
            OrderBy orderBy;

            // Act
            orderBy = new OrderBy(string.Empty);

            // Assert
            Assert.True(orderBy.Ascending);
            Assert.False(orderBy.Descending);
        }

        [Fact]
        public void When_InstantiatingOrderBy_Expect_AscendingFalseAndDescendingTrue()
        {
            // Arrange
            OrderBy orderBy;

            // Act
            orderBy = new OrderBy(string.Empty, false);

            // Assert
            Assert.False(orderBy.Ascending);
            Assert.True(orderBy.Descending);
        }
    }
}
