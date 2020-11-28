namespace Scaffold.Application.UnitTests.Common.Models
{
    using System;
    using System.Collections.Generic;
    using Scaffold.Application.Common.Exceptions;
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
                    .OrderBy(nameof(TestClass.Property1))
                    .ThenBy(nameof(TestClass.Property2))
                    .ThenBy(nameof(TestClass.Property3));

                // Act
                int result = sortOrder.Count;

                // Assert
                Assert.Equal(3, result);
            }
        }

        public class GetEnumerator
        {
            [Fact]
            public void When_GettingEnumerator_Expect_Enumerator()
            {
                // Arrange
                SortOrder<TestClass> sortOrder = SortOrder<TestClass>
                    .OrderBy(nameof(TestClass.Property1));

                // Act
                IEnumerator<(string PropertyName, bool Descending)> result = sortOrder.GetEnumerator();

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
                    .OrderBy(nameof(TestClass.Property1))
                    .ThenBy(nameof(TestClass.Property2))
                    .ThenBy(nameof(TestClass.Property3));

                // Act
                (string propertyName, bool descending) = sortOrder[1];

                // Assert
                Assert.Equal(nameof(TestClass.Property2), propertyName);
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
                    .OrderBy(nameof(TestClass.Property1));

                // Assert
                Assert.NotNull(result);
                Assert.Single(result);
                Assert.Equal(nameof(TestClass.Property1), result[0].PropertyName);
                Assert.False(result[0].Descending);
            }

            [Fact]
            public void When_InitializingSortWithNonExistingProperty_Expect_PropertyNotFoundException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => SortOrder<TestClass>
                    .OrderBy("property1"));

                // Assert
                Assert.IsType<PropertyNotFoundException>(exception);
            }

            [Fact]
            public void When_InitializingSortWithComparableProperty_Expect_InitializedSortOrder()
            {
                // Arrange
                SortOrder<TestClass> result1, result2;

                // Act
                result1 = SortOrder<TestClass>
                    .OrderBy(nameof(TestClass.Property4));

                result2 = SortOrder<TestClass>
                    .OrderBy(nameof(TestClass.Property5));

                // Assert
                Assert.NotNull(result1);
                Assert.Single(result1);
                Assert.Equal(nameof(TestClass.Property4), result1[0].PropertyName);
                Assert.False(result1[0].Descending);

                Assert.NotNull(result2);
                Assert.Single(result2);
                Assert.Equal(nameof(TestClass.Property5), result2[0].PropertyName);
                Assert.False(result2[0].Descending);
            }

            [Fact]
            public void When_InitializingSortWithNonComparableProperty_Expect_PropertyNotComparableException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => SortOrder<TestClass>
                    .OrderBy(nameof(TestClass.Property6)));

                // Assert
                Assert.IsType<PropertyNotComparableException>(exception);
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
                    .OrderByDescending(nameof(TestClass.Property1));

                // Assert
                Assert.NotNull(result);
                Assert.Single(result);
                Assert.Equal(nameof(TestClass.Property1), result[0].PropertyName);
                Assert.True(result[0].Descending);
            }

            [Fact]
            public void When_InitializingSortWithNonExistingProperty_Expect_PropertyNotFoundException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => SortOrder<TestClass>
                    .OrderByDescending("property1"));

                // Assert
                Assert.IsType<PropertyNotFoundException>(exception);
            }

            [Fact]
            public void When_InitializingSortWithComparableProperty_Expect_InitializedSortOrder()
            {
                // Arrange
                SortOrder<TestClass> result1, result2;

                // Act
                result1 = SortOrder<TestClass>
                    .OrderByDescending(nameof(TestClass.Property4));

                result2 = SortOrder<TestClass>
                    .OrderByDescending(nameof(TestClass.Property5));

                // Assert
                Assert.NotNull(result1);
                Assert.Single(result1);
                Assert.Equal(nameof(TestClass.Property4), result1[0].PropertyName);
                Assert.True(result1[0].Descending);

                Assert.NotNull(result2);
                Assert.Single(result2);
                Assert.Equal(nameof(TestClass.Property5), result2[0].PropertyName);
                Assert.True(result2[0].Descending);
            }

            [Fact]
            public void When_InitializingSortWithNonComparableProperty_Expect_PropertyNotComparableException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => SortOrder<TestClass>
                    .OrderByDescending(nameof(TestClass.Property6)));

                // Assert
                Assert.IsType<PropertyNotComparableException>(exception);
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
                    .OrderBy(nameof(TestClass.Property1))
                    .ThenBy(nameof(TestClass.Property2));

                // Assert
                Assert.Collection(
                    result,
                    orderBy =>
                    {
                        Assert.Equal(nameof(TestClass.Property1), orderBy.PropertyName);
                        Assert.False(orderBy.Descending);
                    },
                    thenBy =>
                    {
                        Assert.Equal(nameof(TestClass.Property2), thenBy.PropertyName);
                        Assert.False(thenBy.Descending);
                    });
            }

            [Fact]
            public void When_AddingSecondarySortWithNonExistingProperty_Expect_PropertyNotFoundException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => SortOrder<TestClass>
                    .OrderBy(nameof(TestClass.Property1))
                    .ThenBy("property2"));

                // Assert
                Assert.IsType<PropertyNotFoundException>(exception);
            }

            [Fact]
            public void When_AddingSecondarySortWithComparableProperty_Expect_SecondarySortAddedToSortOrder()
            {
                // Arrange
                SortOrder<TestClass> result;

                // Act
                result = SortOrder<TestClass>
                    .OrderBy(nameof(TestClass.Property1))
                    .ThenBy(nameof(TestClass.Property4))
                    .ThenBy(nameof(TestClass.Property5));

                // Assert
                Assert.Collection(
                    result,
                    orderBy =>
                    {
                        Assert.Equal(nameof(TestClass.Property1), orderBy.PropertyName);
                        Assert.False(orderBy.Descending);
                    },
                    thenBy =>
                    {
                        Assert.Equal(nameof(TestClass.Property4), thenBy.PropertyName);
                        Assert.False(thenBy.Descending);
                    },
                    thenBy =>
                    {
                        Assert.Equal(nameof(TestClass.Property5), thenBy.PropertyName);
                        Assert.False(thenBy.Descending);
                    });
            }

            [Fact]
            public void When_AddingSecondarySortWithNonComparableProperty_Expect_PropertyNotComparableException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => SortOrder<TestClass>
                    .OrderBy(nameof(TestClass.Property1))
                    .ThenBy(nameof(TestClass.Property6)));

                // Assert
                Assert.IsType<PropertyNotComparableException>(exception);
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
                    .OrderBy(nameof(TestClass.Property1))
                    .ThenByDescending(nameof(TestClass.Property2));

                // Assert
                Assert.Collection(
                   result,
                   orderBy =>
                   {
                       Assert.Equal(nameof(TestClass.Property1), orderBy.PropertyName);
                       Assert.False(orderBy.Descending);
                   },
                   thenByDescending =>
                   {
                       Assert.Equal(nameof(TestClass.Property2), thenByDescending.PropertyName);
                       Assert.True(thenByDescending.Descending);
                   });
            }

            [Fact]
            public void When_AddingSecondarySortWithNonExistingProperty_Expect_PropertyNotFoundException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => SortOrder<TestClass>
                    .OrderBy(nameof(TestClass.Property1))
                    .ThenByDescending("property2"));

                // Assert
                Assert.IsType<PropertyNotFoundException>(exception);
            }

            [Fact]
            public void When_AddingSecondarySortWithComparableProperty_Expect_SecondarySortAddedToSortOrder()
            {
                // Arrange
                SortOrder<TestClass> result;

                // Act
                result = SortOrder<TestClass>
                    .OrderBy(nameof(TestClass.Property1))
                    .ThenByDescending(nameof(TestClass.Property4))
                    .ThenByDescending(nameof(TestClass.Property5));

                // Assert
                Assert.Collection(
                    result,
                    orderBy =>
                    {
                        Assert.Equal(nameof(TestClass.Property1), orderBy.PropertyName);
                        Assert.False(orderBy.Descending);
                    },
                    thenByDescending =>
                    {
                        Assert.Equal(nameof(TestClass.Property4), thenByDescending.PropertyName);
                        Assert.True(thenByDescending.Descending);
                    },
                    thenByDescending =>
                    {
                        Assert.Equal(nameof(TestClass.Property5), thenByDescending.PropertyName);
                        Assert.True(thenByDescending.Descending);
                    });
            }

            [Fact]
            public void When_AddingSecondarySortWithNonComparableProperty_Expect_PropertyNotComparableException()
            {
                // Arrange
                Exception exception;

                // Act
                exception = Record.Exception(() => SortOrder<TestClass>
                    .OrderBy(nameof(TestClass.Property1))
                    .ThenByDescending(nameof(TestClass.Property6)));

                // Assert
                Assert.IsType<PropertyNotComparableException>(exception);
            }
        }

        private class TestClass
        {
            public int Property1 { get; set; } = new Random().Next();

            public string Property2 { get; set; } = string.Empty;

            public string Property3 { get; set; } = string.Empty;

            public ComparableProperty1 Property4 { get; set; } = new ComparableProperty1();

            public ComparableProperty2 Property5 { get; set; } = new ComparableProperty2();

            public NonComparableProperty Property6 { get; set; } = new NonComparableProperty();
        }

        private class ComparableProperty1 : IComparable<ComparableProperty1>
        {
            public int Value { get; set; } = new Random().Next();

            public int CompareTo(ComparableProperty1 other)
            {
                return this.Value.CompareTo(other.Value);
            }
        }

        private class ComparableProperty2 : IComparable
        {
            public int Value { get; set; } = new Random().Next();

            public int CompareTo(object obj)
            {
                if (obj is ComparableProperty2 other)
                {
                    return this.Value.CompareTo(other.Value);
                }

                throw new ArgumentException("Object is not comparable.");
            }
        }

        private class NonComparableProperty
        {
        }
    }
}
