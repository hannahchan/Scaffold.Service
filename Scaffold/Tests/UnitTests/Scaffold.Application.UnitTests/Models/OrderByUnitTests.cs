namespace Scaffold.Application.UnitTests.Models
{
    using Scaffold.Application.Models;
    using Xunit;

    public static class OrderByUnitTests
    {
        public class Constructor
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

        public class Ascending
        {
            [Fact]
            public void When_SettingAscendingToTrue_Expect_AscendingTrueAndDescendingFalse()
            {
                // Act
                OrderBy orderBy = new OrderBy(string.Empty) { Ascending = true };

                // Assert
                Assert.True(orderBy.Ascending);
                Assert.False(orderBy.Descending);
            }

            [Fact]
            public void When_SettingAscendingToFalse_Expect_AscendingFalseAndDescendingTrue()
            {
                // Act
                OrderBy orderBy = new OrderBy(string.Empty) { Ascending = false };

                // Assert
                Assert.False(orderBy.Ascending);
                Assert.True(orderBy.Descending);
            }
        }

        public class Descending
        {
            [Fact]
            public void When_SettingDescendingToTrue_Expect_AscendingFalseAndDescendingTrue()
            {
                // Act
                OrderBy orderBy = new OrderBy(string.Empty) { Descending = true };

                // Assert
                Assert.False(orderBy.Ascending);
                Assert.True(orderBy.Descending);
            }

            [Fact]
            public void When_SettingDescendingToFalse_Expect_AscendingTrueAndDescendingFalse()
            {
                // Act
                OrderBy orderBy = new OrderBy(string.Empty) { Descending = false };

                // Assert
                Assert.True(orderBy.Ascending);
                Assert.False(orderBy.Descending);
            }
        }
    }
}
