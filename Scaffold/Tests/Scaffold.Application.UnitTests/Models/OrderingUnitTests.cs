namespace Scaffold.Application.UnitTests.Models
{
    using System;
    using System.Collections.Generic;
    using Scaffold.Application.Exceptions;
    using Scaffold.Application.Models;
    using Xunit;

    public static class OrderingUnitTests
    {
        public class Add
        {
            [Fact]
            public void When_AddingOrderBy_Expect_OrderByAdded()
            {
                // Act
                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    new OrderBy(nameof(TestClass.Property1)),
                    new OrderBy(nameof(TestClass.Property2)),
                    new OrderBy(nameof(TestClass.Property3)),
                };

                // Assert
                Assert.Equal(nameof(TestClass.Property1), ordering[0].PropertyName);
                Assert.Equal(nameof(TestClass.Property2), ordering[1].PropertyName);
                Assert.Equal(nameof(TestClass.Property3), ordering[2].PropertyName);
            }

            [Fact]
            public void When_AddingOrderByWithNonExistingProperty_Expect_PropertyNotFoundException()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>();

                // Act
                Exception exception = Record.Exception(() => ordering.Add(new OrderBy("property1")));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<PropertyNotFoundException>(exception);
            }

            [Fact]
            public void When_AddingOrderByWithComparableProperty_Expect_OrderByAdded()
            {
                // Act
                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    new OrderBy(nameof(TestClass.Property4)),
                    new OrderBy(nameof(TestClass.Property5)),
                };

                // Assert
                Assert.Equal(nameof(TestClass.Property4), ordering[0].PropertyName);
                Assert.Equal(nameof(TestClass.Property5), ordering[1].PropertyName);
            }

            [Fact]
            public void When_AddingOrderByWithNonComparableProperty_Expect_PropertyNotComparableException()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>();

                // Act
                Exception exception = Record.Exception(() => ordering.Add(new OrderBy(nameof(TestClass.Property6))));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<PropertyNotComparableException>(exception);
            }
        }

        public class Clear
        {
            [Fact]
            public void When_ClearingList_Expect_EmptyList()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    new OrderBy(nameof(TestClass.Property1)),
                    new OrderBy(nameof(TestClass.Property2)),
                    new OrderBy(nameof(TestClass.Property3)),
                };

                // Act
                ordering.Clear();

                // Assert
                Assert.Empty(ordering);
            }
        }

        public class Contains
        {
            [Fact]
            public void When_OrderingContainsOrderBy_Expect_True()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>();
                OrderBy orderby = new OrderBy(nameof(TestClass.Property1));
                ordering.Add(orderby);

                // Act
                bool result = ordering.Contains(orderby);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void When_OrderingDoesNotContainOrderBy_Expect_False()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>();
                OrderBy orderby = new OrderBy(nameof(TestClass.Property1));

                // Act
                bool result = ordering.Contains(orderby);

                // Assert
                Assert.False(result);
            }
        }

        public class CopyTo
        {
            [Fact]
            public void When_CopyingToArray_Expect_CopiedToArray()
            {
                // Arrange
                OrderBy orderBy1 = new OrderBy(nameof(TestClass.Property1));
                OrderBy orderBy2 = new OrderBy(nameof(TestClass.Property2));
                OrderBy orderBy3 = new OrderBy(nameof(TestClass.Property3));

                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    orderBy1,
                    orderBy2,
                    orderBy3,
                };

                OrderBy[] array = new OrderBy[5];

                // Act
                ordering.CopyTo(array, 1);

                // Assert
                Assert.Null(array[0]);
                Assert.Equal(orderBy1, array[1]);
                Assert.Equal(orderBy2, array[2]);
                Assert.Equal(orderBy3, array[3]);
                Assert.Null(array[4]);
            }
        }

        public class Count
        {
            [Fact]
            public void When_GettingCount_Expect_Count()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    new OrderBy(nameof(TestClass.Property1)),
                    new OrderBy(nameof(TestClass.Property2)),
                    new OrderBy(nameof(TestClass.Property3)),
                };

                // Act
                int result = ordering.Count;

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
                Ordering<TestClass> ordering = new Ordering<TestClass>();

                // Act
                IEnumerator<OrderBy> result = ordering.GetEnumerator();

                // Assert
                Assert.NotNull(result);
            }
        }

        public class Indexer
        {
            [Fact]
            public void When_GettingOrderByAtIndex_Expect_OrderByAtIndex()
            {
                // Arrange
                OrderBy orderBy1 = new OrderBy(nameof(TestClass.Property1));
                OrderBy orderBy2 = new OrderBy(nameof(TestClass.Property2));
                OrderBy orderBy3 = new OrderBy(nameof(TestClass.Property3));

                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    orderBy1,
                    orderBy2,
                    orderBy3,
                };

                // Act
                OrderBy result = ordering[1];

                // Assert
                Assert.Equal(orderBy2, result);
            }

            [Fact]
            public void When_SettingOrderByAtIndex_Expect_OrderBySetAtIndex()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    new OrderBy(nameof(TestClass.Property1)),
                    new OrderBy(nameof(TestClass.Property2)),
                };

                OrderBy orderBy = new OrderBy(nameof(TestClass.Property3));

                // Act
                ordering[1] = orderBy;

                // Assert
                Assert.Equal(orderBy, ordering[1]);
                Assert.Equal(nameof(TestClass.Property1), ordering[0].PropertyName);
                Assert.Equal(nameof(TestClass.Property3), ordering[1].PropertyName);
            }

            [Fact]
            public void When_SettingOrderByWithNonExistingProperty_Expect_PropertyNotFoundException()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>();

                // Act
                Exception exception = Record.Exception(() => ordering[0] = new OrderBy("property1"));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<PropertyNotFoundException>(exception);
            }

            [Fact]
            public void When_SettingOrderByAtIndexWithComparableProperty_Expect_OrderBySetAtIndex()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    new OrderBy(nameof(TestClass.Property1)),
                    new OrderBy(nameof(TestClass.Property2)),
                };

                OrderBy orderBy1 = new OrderBy(nameof(TestClass.Property4));
                OrderBy orderBy2 = new OrderBy(nameof(TestClass.Property5));

                // Act
                ordering[0] = orderBy1;
                ordering[1] = orderBy2;

                // Assert
                Assert.Equal(orderBy1, ordering[0]);
                Assert.Equal(orderBy2, ordering[1]);
                Assert.Equal(nameof(TestClass.Property4), ordering[0].PropertyName);
                Assert.Equal(nameof(TestClass.Property5), ordering[1].PropertyName);
            }

            [Fact]
            public void When_SettingOrderByAtIndexWithNonComparableProperty_Expect_PropertyNotComparableException()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>();

                // Act
                Exception exception = Record.Exception(() => ordering[0] = new OrderBy(nameof(TestClass.Property6)));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<PropertyNotComparableException>(exception);
            }
        }

        public class IndexOf
        {
            [Fact]
            public void When_GettingIndexOfOrderBy_Expect_Index()
            {
                // Arrange
                OrderBy orderBy1 = new OrderBy(nameof(TestClass.Property1));
                OrderBy orderBy2 = new OrderBy(nameof(TestClass.Property2));
                OrderBy orderBy3 = new OrderBy(nameof(TestClass.Property3));

                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    orderBy1,
                    orderBy2,
                    orderBy3,
                };

                // Act
                int result = ordering.IndexOf(orderBy3);

                // Assert
                Assert.Equal(2, result);
            }
        }

        public class Insert
        {
            [Fact]
            public void When_InsertingOrderByAtIndex_Expect_OrderByInsertedAtIndex()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    new OrderBy(nameof(TestClass.Property1)),
                    new OrderBy(nameof(TestClass.Property3)),
                };

                // Act
                ordering.Insert(1, new OrderBy(nameof(TestClass.Property2)));

                // Assert
                Assert.Equal(nameof(TestClass.Property1), ordering[0].PropertyName);
                Assert.Equal(nameof(TestClass.Property2), ordering[1].PropertyName);
                Assert.Equal(nameof(TestClass.Property3), ordering[2].PropertyName);
            }

            [Fact]
            public void When_InsertingOrderByWithNonExistingProperty_Expect_PropertyNotFoundException()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>();

                // Act
                Exception exception = Record.Exception(() => ordering.Insert(0, new OrderBy("property1")));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<PropertyNotFoundException>(exception);
            }

            [Fact]
            public void When_InsertingOrderByAtIndexWithComparableProperty_Expect_OrderByInsertedAtIndex()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    new OrderBy(nameof(TestClass.Property1)),
                    new OrderBy(nameof(TestClass.Property3)),
                };

                // Act
                ordering.Insert(1, new OrderBy(nameof(TestClass.Property4)));
                ordering.Insert(2, new OrderBy(nameof(TestClass.Property5)));

                // Assert
                Assert.Equal(nameof(TestClass.Property1), ordering[0].PropertyName);
                Assert.Equal(nameof(TestClass.Property4), ordering[1].PropertyName);
                Assert.Equal(nameof(TestClass.Property5), ordering[2].PropertyName);
                Assert.Equal(nameof(TestClass.Property3), ordering[3].PropertyName);
            }

            [Fact]
            public void When_InsertingOrderByWithNonComparableProperty_Expect_PropertyNotComparableException()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>();

                // Act
                Exception exception = Record.Exception(() => ordering.Insert(0, new OrderBy(nameof(TestClass.Property6))));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<PropertyNotComparableException>(exception);
            }
        }

        public class IsReadOnly
        {
            [Fact]
            public void When_GettingIsReadOnly_Expect_False()
            {
                // Arrange
                Ordering<TestClass> ordering = new Ordering<TestClass>();

                // Act
                bool result = ordering.IsReadOnly;

                // Assert
                Assert.False(result);
            }
        }

        public class Remove
        {
            [Fact]
            public void When_RemovingOrderBy_Expect_OrderByRemoved()
            {
                // Arrange
                OrderBy orderBy1 = new OrderBy(nameof(TestClass.Property1));
                OrderBy orderBy2 = new OrderBy(nameof(TestClass.Property2));
                OrderBy orderBy3 = new OrderBy(nameof(TestClass.Property3));

                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    orderBy1,
                    orderBy2,
                    orderBy3,
                };

                // Act
                ordering.Remove(orderBy2);

                // Assert
                Assert.DoesNotContain(orderBy2, ordering);
                Assert.Equal(2, ordering.Count);
                Assert.Equal(orderBy1, ordering[0]);
                Assert.Equal(orderBy3, ordering[1]);
            }
        }

        public class RemoveAt
        {
            [Fact]
            public void When_RemovingOrderByAtIndex_Expect_OrderByRemovedAtIndex()
            {
                // Arrange
                OrderBy orderBy1 = new OrderBy(nameof(TestClass.Property1));
                OrderBy orderBy2 = new OrderBy(nameof(TestClass.Property2));
                OrderBy orderBy3 = new OrderBy(nameof(TestClass.Property3));

                Ordering<TestClass> ordering = new Ordering<TestClass>
                {
                    orderBy1,
                    orderBy2,
                    orderBy3,
                };

                // Act
                ordering.RemoveAt(1);

                // Assert
                Assert.DoesNotContain(orderBy2, ordering);
                Assert.Equal(2, ordering.Count);
                Assert.Equal(orderBy1, ordering[0]);
                Assert.Equal(orderBy3, ordering[1]);
            }
        }

        private class TestClass
        {
            public int Property1 { get; set; } = 0;

            public string Property2 { get; set; } = string.Empty;

            public string Property3 { get; set; } = string.Empty;

            public ComparableProperty1 Property4 { get; set; } = new ComparableProperty1();

            public ComparableProperty2 Property5 { get; set; } = new ComparableProperty2();

            public NonComparableProperty Property6 { get; set; } = new NonComparableProperty();
        }

        private class ComparableProperty1 : IComparable<ComparableProperty1>
        {
            public int Value { get; set; } = 0;

            public int CompareTo(ComparableProperty1 other)
            {
                return this.Value.CompareTo(other.Value);
            }
        }

        private class ComparableProperty2 : IComparable
        {
            public int Value { get; set; } = 0;

            public int CompareTo(object? obj)
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
