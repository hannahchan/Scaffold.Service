namespace Scaffold.WebApi.Extensions
{
    using System;
    using System.Linq.Expressions;

    public static class PredicateExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            InvocationExpression invocationExpression = Expression.Invoke(expression2, expression1.Parameters);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression1.Body, invocationExpression), expression1.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            InvocationExpression invocationExpression = Expression.Invoke(expression2, expression1.Parameters);

            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expression1.Body, invocationExpression), expression1.Parameters);
        }
    }
}
