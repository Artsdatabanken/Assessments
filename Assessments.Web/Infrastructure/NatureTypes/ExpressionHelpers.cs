using System.Linq.Expressions;

namespace Assessments.Web.Infrastructure.NatureTypes;

public static class ExpressionHelpers
{
    public static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        switch (left)
        {
            case null when right == null:
                throw new ArgumentException("At least one argument must not be null");
            case null:
                return right;
        }

        if (right == null) 
            return left;

        var parameter = Expression.Parameter(typeof(T), "p");
        var combined = new ParameterReplacer(parameter).Visit(Expression.OrElse(left.Body, right.Body));

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        internal ParameterReplacer(ParameterExpression parameter) => _parameter = parameter;

        protected override Expression VisitParameter(ParameterExpression node) => _parameter;
    }
}