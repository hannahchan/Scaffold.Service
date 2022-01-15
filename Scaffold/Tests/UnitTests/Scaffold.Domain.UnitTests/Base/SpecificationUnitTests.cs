namespace Scaffold.Domain.UnitTests.Base;

using System;
using System.Collections.Generic;
using Scaffold.Domain.Base;
using Xunit;

public class SpecificationUnitTests
{
    public static readonly TheoryData<Specification<Person>, string, Person, Person> TestSpecifications =
        new TheoryData<Specification<Person>, string, Person, Person>
        {
            // Simple Specification
            {
                new AtLeast16YearsSpecification(),
                "person => (person.Age >= 16)",
                new Person { Age = 16 },
                new Person { Age = 15 }
            },

            // Complex Specification with 1 Nested Specification
            {
                new NestedAtLeast16YearsSpecification(),
                "person => (person.Age >= 16)",
                new Person { Age = 16 },
                new Person { Age = 15 }
            },

            // Complex Specification with 2 Nested Specification
            {
                new AtLeast16YearsAndLikesBlueSpecification(),
                "person => ((person.Age >= 16) AndAlso (person.FavoriteColor == \"Blue\"))",
                new Person { Age = 16, FavoriteColor = "Blue" },
                new Person { Age = 16, FavoriteColor = "Yellow" }
            },

            // Complex Specification with 3 Nested Specification
            {
                new AtLeast16YearsLikesBlueAndChocolateSpecification(),
                "person => (((person.Age >= 16) AndAlso (person.FavoriteColor == \"Blue\")) AndAlso (person.FavoriteSnack == \"Chocolate\"))",
                new Person { Age = 16, FavoriteColor = "Blue", FavoriteSnack = "Chocolate" },
                new Person { Age = 16, FavoriteColor = "Blue", FavoriteSnack = "Potato Chips" }
            },
        };

    [Theory]
    [MemberData(nameof(TestSpecifications))]
    public void When_UsingSpecification_Expect_WorkingSpecification(Specification<Person> specification, string expectedExpression, Person isSatisfiedBy, Person isNotSatisfiedBy)
    {
        // Act and Assert
        Assert.Equal(expectedExpression, specification.GetExpression().ToString());
        Assert.True(specification.IsSatisfiedBy(isSatisfiedBy));
        Assert.False(specification.IsSatisfiedBy(isNotSatisfiedBy));
    }

    [Fact]
    public void When_UsingEmptySpecification_Expect_WorkingSpecification()
    {
        // Arrange
        Person person = new Person();
        EmptySpecification specification = new EmptySpecification();

        // Act and Assert
        Assert.Equal("person => False", specification.GetExpression().ToString());
        Assert.False(specification.IsSatisfiedBy(person));
    }

    public class Person
    {
        public int Age { get; set; }

        public string FavoriteColor { get; set; }

        public string FavoriteSnack { get; set; }
    }

    private class AtLeast16YearsSpecification : Specification<Person>
    {
        public AtLeast16YearsSpecification()
            : base(person => person.Age >= 16)
        {
        }
    }

    private class FavoriteColorIsBlueSpecification : Specification<Person>
    {
        public FavoriteColorIsBlueSpecification()
            : base(person => person.FavoriteColor == "Blue")
        {
        }
    }

    private class FavoriteSnackIsChocolateSpecification : Specification<Person>
    {
        public FavoriteSnackIsChocolateSpecification()
            : base(person => person.FavoriteSnack == "Chocolate")
        {
        }
    }

    private class EmptySpecification : Specification<Person>
    {
        public EmptySpecification()
            : base(Array.Empty<Specification<Person>>(), "person")
        {
        }
    }

    private class NestedAtLeast16YearsSpecification : Specification<Person>
    {
        public NestedAtLeast16YearsSpecification()
            : base(new[] { new AtLeast16YearsSpecification() }, "person")
        {
        }
    }

    private class AtLeast16YearsAndLikesBlueSpecification : Specification<Person>
    {
        public AtLeast16YearsAndLikesBlueSpecification()
            : base(Specifications(), "person")
        {
        }

        private static IEnumerable<Specification<Person>> Specifications()
        {
            return new Specification<Person>[]
            {
                new AtLeast16YearsSpecification(),
                new FavoriteColorIsBlueSpecification(),
            };
        }
    }

    private class AtLeast16YearsLikesBlueAndChocolateSpecification : Specification<Person>
    {
        public AtLeast16YearsLikesBlueAndChocolateSpecification()
            : base(Specifications(), "person")
        {
        }

        private static IEnumerable<Specification<Person>> Specifications()
        {
            return new Specification<Person>[]
            {
                new AtLeast16YearsSpecification(),
                new FavoriteColorIsBlueSpecification(),
                new FavoriteSnackIsChocolateSpecification(),
            };
        }
    }
}
