namespace Scaffold.Domain.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public abstract class Specification<T> : ISpecification<T>
{
    private readonly Func<T, bool> function;

    protected Specification(Expression<Func<T, bool>> expression)
    {
        this.Expression = expression;
        this.function = expression.Compile();
    }

    protected Specification(IEnumerable<Specification<T>> specifications, string? parameterName = null)
    {
        ParameterExpression parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), parameterName);

        if (specifications.Any())
        {
            ReplaceParameterVisitor visitor = new ReplaceParameterVisitor(parameter);

            Queue<Expression> expressionBodies = new Queue<Expression>(specifications
                .Select(specification => visitor.Visit(specification.Expression.Body))
                .ToList());

            Expression expressionBody = expressionBodies.Dequeue();

            while (expressionBodies.Count > 0)
            {
                expressionBody = System.Linq.Expressions.Expression.AndAlso(expressionBody, expressionBodies.Dequeue());
            }

            this.Expression = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(expressionBody, parameter);
        }
        else
        {
            this.Expression = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.Constant(false), parameter);
        }

        this.function = this.Expression.Compile();
    }

    public Expression<Func<T, bool>> Expression { get; }

    public bool IsSatisfiedBy(T obj)
    {
        return this.function(obj);
    }

    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression newParameter;

        public ReplaceParameterVisitor(ParameterExpression newParameter)
        {
            this.newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return this.newParameter;
        }
    }
}
