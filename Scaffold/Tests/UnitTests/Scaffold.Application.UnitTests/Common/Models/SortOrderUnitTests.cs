namespace Scaffold.Application.UnitTests.Common.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Scaffold.Application.Common.Models;
using Xunit;

public static class SortOrderUnitTests
{
    public class Count
    {
        [Fact]
        public void When_GettingCount_Expect_Count()
        {
            // Arrange
            SortOrder<TestClass> sortOrder = SortOrder<TestClass>
                .OrderBy(testObject => testObject.Property1)
                .ThenBy(testObject => testObject.Property2)
                .ThenBy(testObject => testObject.Property3);

            // Act
            int result = sortOrder.Count;

            // Assert
            Assert.Equal(3, result);
        }
    }

    public class GetEnumerator
    {
        [Fact]
        public void When_GettingGenericEnumerator_Expect_Enumerator()
        {
            // Arrange
            IEnumerable<(LambdaExpression KeySelector, bool Descending)> sortOrder = SortOrder<TestClass>
                .OrderBy(testObject => testObject.Property1);

            // Act
            using IEnumerator<(LambdaExpression KeySelector, bool Descending)> result = sortOrder.GetEnumerator();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void When_GettingNonGenericEnumerator_Expect_Enumerator()
        {
            // Arrange
            IEnumerable sortOrder = SortOrder<TestClass>
                .OrderBy(testObject => testObject.Property1);

            // Act
            IEnumerator result = sortOrder.GetEnumerator();

            // Assert
            Assert.NotNull(result);
        }
    }

    public class Indexer
    {
        [Fact]
        public void When_GettingSortItemAtIndex_Expect_SortItemAtIndex()
        {
            // Arrange
            SortOrder<TestClass> sortOrder = SortOrder<TestClass>
                .OrderBy(testObject => testObject.Property1)
                .ThenBy(testObject => testObject.Property2)
                .ThenBy(testObject => testObject.Property3);

            // Act
            (LambdaExpression keySelector, bool descending) = sortOrder[1];

            // Assert
            Assert.Equal("testObject => testObject.Property2", keySelector.ToString());
            Assert.False(descending);
        }
    }

    public class OrderBy
    {
        [Fact]
        public void When_InitializingSort_Expect_InitializedSortOrder()
        {
            // Arrange
            SortOrder<TestClass> result;

            // Act
            result = SortOrder<TestClass>
                .OrderBy(testObject => testObject.Property1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("testObject => testObject.Property1", result[0].KeySelector.ToString());
            Assert.False(result[0].Descending);
        }

        [Fact]
        public void When_InitializingSortWithComparableProperty_Expect_InitializedSortOrder()
        {
            // Arrange
            SortOrder<TestClass> result;

            // Act
            result = SortOrder<TestClass>
                .OrderBy(testObject => testObject.Property4);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("testObject => testObject.Property4", result[0].KeySelector.ToString());
            Assert.False(result[0].Descending);
        }
    }

    public class OrderByDescending
    {
        [Fact]
        public void When_InitializingSort_Expect_InitializedSortOrder()
        {
            // Arrange
            SortOrder<TestClass> result;

            // Act
            result = SortOrder<TestClass>
                .OrderByDescending(testObject => testObject.Property1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("testObject => testObject.Property1", result[0].KeySelector.ToString());
            Assert.True(result[0].Descending);
        }

        [Fact]
        public void When_InitializingSortWithComparableProperty_Expect_InitializedSortOrder()
        {
            // Arrange
            SortOrder<TestClass> result;

            // Act
            result = SortOrder<TestClass>
                .OrderByDescending(testObject => testObject.Property4);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("testObject => testObject.Property4", result[0].KeySelector.ToString());
            Assert.True(result[0].Descending);
        }
    }

    public class ThenBy
    {
        [Fact]
        public void When_AddingSecondarySort_Expect_SecondarySortAddedToSortOrder()
        {
            // Arrange
            SortOrder<TestClass> result;

            // Act
            result = SortOrder<TestClass>
                .OrderBy(testObject => testObject.Property1)
                .ThenBy(testObject => testObject.Property2);

            // Assert
            Assert.Collection(
                result,
                orderBy =>
                {
                    Assert.Equal("testObject => testObject.Property1", orderBy.KeySelector.ToString());
                    Assert.False(orderBy.Descending);
                },
                thenBy =>
                {
                    Assert.Equal("testObject => testObject.Property2", thenBy.KeySelector.ToString());
                    Assert.False(thenBy.Descending);
                });
        }

        [Fact]
        public void When_AddingSecondarySortWithComparableProperty_Expect_SecondarySortAddedToSortOrder()
        {
            // Arrange
            SortOrder<TestClass> result;

            // Act
            result = SortOrder<TestClass>
                .OrderBy(testObject => testObject.Property1)
                .ThenBy(testObject => testObject.Property4);

            // Assert
            Assert.Collection(
                result,
                orderBy =>
                {
                    Assert.Equal("testObject => testObject.Property1", orderBy.KeySelector.ToString());
                    Assert.False(orderBy.Descending);
                },
                thenBy =>
                {
                    Assert.Equal("testObject => testObject.Property4", thenBy.KeySelector.ToString());
                    Assert.False(thenBy.Descending);
                });
        }
    }

    public class ThenByDescending
    {
        [Fact]
        public void When_AddingSecondarySort_Expect_SecondarySortAddedToSortOrder()
        {
            // Arrange
            SortOrder<TestClass> result;

            // Act
            result = SortOrder<TestClass>
                .OrderBy(testObject => testObject.Property1)
                .ThenByDescending(testObject => testObject.Property2);

            // Assert
            Assert.Collection(
               result,
               orderBy =>
               {
                   Assert.Equal("testObject => testObject.Property1", orderBy.KeySelector.ToString());
                   Assert.False(orderBy.Descending);
               },
               thenByDescending =>
               {
                   Assert.Equal("testObject => testObject.Property2", thenByDescending.KeySelector.ToString());
                   Assert.True(thenByDescending.Descending);
               });
        }

        [Fact]
        public void When_AddingSecondarySortWithComparableProperty_Expect_SecondarySortAddedToSortOrder()
        {
            // Arrange
            SortOrder<TestClass> result;

            // Act
            result = SortOrder<TestClass>
                .OrderBy(testObject => testObject.Property1)
                .ThenByDescending(testObject => testObject.Property4);

            // Assert
            Assert.Collection(
                result,
                orderBy =>
                {
                    Assert.Equal("testObject => testObject.Property1", orderBy.KeySelector.ToString());
                    Assert.False(orderBy.Descending);
                },
                thenByDescending =>
                {
                    Assert.Equal("testObject => testObject.Property4", thenByDescending.KeySelector.ToString());
                    Assert.True(thenByDescending.Descending);
                });
        }
    }

    private class TestClass
    {
        public int Property1 { get; set; } = new Random().Next();

        public string Property2 { get; set; } = string.Empty;

        public string Property3 { get; set; } = string.Empty;

        public ComparableProperty Property4 { get; set; } = new ComparableProperty();
    }

    private class ComparableProperty : IComparable<ComparableProperty>
    {
        public int Value { get; set; } = new Random().Next();

        public int CompareTo(ComparableProperty other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
}
