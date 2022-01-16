namespace Scaffold.Domain.UnitTests.Base;

using System;
using System.Collections.Generic;
using Scaffold.Domain.Base;
using Xunit;

public class SpecificationUnitTests
{
    public static readonly TheoryData<Specification<Person>, Person, string, bool> TestSpecifications =
        new TheoryData<Specification<Person>, Person, string, bool>
        {
            // Simple Specification
            {
                new AtLeast16YearsSpecification(),
                new Person { Age = 16 },
                "person => (person.Age >= 16)",
                true
            },
            {
                new AtLeast16YearsSpecification(),
                new Person { Age = 15 },
                "person => (person.Age >= 16)",
                false
            },

            // Complex Specification with 0 Nested Specification
            {
                new EmptySpecification(),
                new Person(),
                "person => False",
                false
            },

            // Complex Specification with 1 Nested Specification
            {
                new NestedAtLeast16YearsSpecification(),
                new Person { Age = 16 },
                "person => (person.Age >= 16)",
                true
            },
            {
                new NestedAtLeast16YearsSpecification(),
                new Person { Age = 15 },
                "person => (person.Age >= 16)",
                false
            },

            // Complex Specification with 2 Nested Specifications
            {
                new AtLeast16YearsAndLikesBlueSpecification(),
                new Person { Age = 16, FavoriteColor = "Blue" },
                "person => ((person.Age >= 16) AndAlso (person.FavoriteColor == \"Blue\"))",
                true
            },
            {
                new AtLeast16YearsAndLikesBlueSpecification(),
                new Person { Age = 16, FavoriteColor = "Yellow" },
                "person => ((person.Age >= 16) AndAlso (person.FavoriteColor == \"Blue\"))",
                false
            },

            // Complex Specification with 3 Nested Specifications
            {
                new AtLeast16YearsLikesBlueAndChocolateSpecification(),
                new Person { Age = 16, FavoriteColor = "Blue", FavoriteSnack = "Chocolate" },
                "person => (((person.Age >= 16) AndAlso (person.FavoriteColor == \"Blue\")) AndAlso (person.FavoriteSnack == \"Chocolate\"))",
                true
            },
            {
                new AtLeast16YearsLikesBlueAndChocolateSpecification(),
                new Person { Age = 16, FavoriteColor = "Blue", FavoriteSnack = "Potato Chips" },
                "person => (((person.Age >= 16) AndAlso (person.FavoriteColor == \"Blue\")) AndAlso (person.FavoriteSnack == \"Chocolate\"))",
                false
            },
        };

    [Theory]
    [MemberData(nameof(TestSpecifications))]
    public void When_UsingSpecification_Expect_WorkingSpecification(Specification<Person> specification, Person person, string expectedExpression, bool expectSatisfiedByPerson)
    {
        // Act and Assert
        Assert.Equal(expectedExpression, specification.Expression.ToString());

        if (expectSatisfiedByPerson)
        {
            Assert.True(specification.IsSatisfiedBy(person));
        }
        else
        {
            Assert.False(specification.IsSatisfiedBy(person));
        }
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
