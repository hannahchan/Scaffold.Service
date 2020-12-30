namespace Scaffold.WebApi.UnitTests.Extensions
{
    using System;
    using System.Linq.Expressions;
    using Scaffold.WebApi.Extensions;
    using Xunit;

    public class PredicateExtensionsUnitTests
    {
        [Fact]
        public void When_CombiningTwoPredicatesWithAnd_Expect_LambdaExpression()
        {
            // Arrange
            Expression<Func<int, bool>> predicate1 = x => x > 2;
            Expression<Func<int, bool>> predicate2 = y => y < 8;

            // Act
            Expression<Func<int, bool>> result = predicate1.And(predicate2);

            // Assert
            Assert.Equal(ExpressionType.Lambda, result.NodeType);
            Assert.Equal(predicate1.Parameters, result.Parameters);

            BinaryExpression binaryExpression = Assert.IsAssignableFrom<BinaryExpression>(result.Body);
            Assert.Equal(ExpressionType.AndAlso, binaryExpression.NodeType);
            Assert.Equal(predicate1.Body, binaryExpression.Left);

            InvocationExpression invocationExpression = Assert.IsAssignableFrom<InvocationExpression>(binaryExpression.Right);
            Assert.Equal(ExpressionType.Invoke, invocationExpression.NodeType);
            Assert.Equal(predicate1.Parameters, invocationExpression.Arguments);
            Assert.Equal(predicate2, invocationExpression.Expression);
        }

        [Fact]
        public void When_CombiningTwoPredicatesWithOr_Expect_LambdaExpression()
        {
            // Arrange
            Expression<Func<int, bool>> predicate1 = x => x < 2;
            Expression<Func<int, bool>> predicate2 = y => y > 8;

            // Act
            Expression<Func<int, bool>> result = predicate1.Or(predicate2);

            // Assert
            Assert.Equal(ExpressionType.Lambda, result.NodeType);
            Assert.Equal(predicate1.Parameters, result.Parameters);

            BinaryExpression binaryExpression = Assert.IsAssignableFrom<BinaryExpression>(result.Body);
            Assert.Equal(ExpressionType.OrElse, binaryExpression.NodeType);
            Assert.Equal(predicate1.Body, binaryExpression.Left);

            InvocationExpression invocationExpression = Assert.IsAssignableFrom<InvocationExpression>(binaryExpression.Right);
            Assert.Equal(ExpressionType.Invoke, invocationExpression.NodeType);
            Assert.Equal(predicate1.Parameters, invocationExpression.Arguments);
            Assert.Equal(predicate2, invocationExpression.Expression);
        }
    }
}
