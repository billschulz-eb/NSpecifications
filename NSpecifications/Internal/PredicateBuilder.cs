using CSharpFunctionalExtensions;
using static System.Linq.Expressions.Expression;

namespace NSpecifications.Internal;

internal static class PredicateBuilder
{
    public static Expression<Func<TTo, Result>> Cast<TFrom, TTo>(Expression<Func<TFrom, Result>> predicate) where TTo : TFrom
    {
        var map = predicate.Parameters.ToDictionary(x => x, x => Parameter(typeof(TTo), x.Name));
        var body = ParameterRebinder.Rebind(predicate.Body, map);
        return Lambda<Func<TTo, Result>>(body, map.Values);
    }

    public static Expression<Func<T, Result>> Not<T>(this Expression<Func<T, Result>> predicate)
        => Lambda<Func<T, Result>>(Expression.Not(predicate.Body), predicate.Parameters);

    public static Expression<Func<T, Result>> And<T>(this Expression<Func<T, Result>> left, Expression<Func<T, Result>> right)
        => left.Merge(right, AndAlso);

    public static Expression<Func<T, Result>> Or<T>(this Expression<Func<T, Result>> left, Expression<Func<T, Result>> right)
        => left.Merge(right, OrElse);

    private static Expression<TDelegate> Merge<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right, Func<Expression, Expression, Expression> merger)
    {
        // Zip parameters (map from parameters of right to parameters of left)
        var map = left.Parameters
            .Zip(right.Parameters, (l, r) => new
            {
                Left = l,
                Right = r
            })
            .ToDictionary(x => x.Right, x => x.Left);

        // Replace parameters in the right lambda expression with the parameters in the left
        var rightBody = ParameterRebinder.Rebind(right.Body, map);

        // Create a merged lambda expression with parameters from the left expression
        return Lambda<TDelegate>(merger(left.Body, rightBody), left.Parameters);
    }
}
