﻿using CSharpFunctionalExtensions;

namespace NSpecifications;

/// <summary>
/// Provides extension methods for <see cref="ISpecification{T}"/> objects.
/// </summary>
public static class SpecificationExtensions
{
    /// <summary>
    /// Determines whether a candidate object satisfies a specification.
    /// </summary>
    /// <typeparam name="T">The type of the candidate object.</typeparam>
    /// <param name="candidate">The value to check.</param>
    /// <param name="spec">The specification to check.</param>
    /// <returns>
    /// <see langword="true"/> if the candidate object satisfies the specification;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static UnitResult<IError> Is<T>(this T candidate, ISpecification<T> spec)
        => spec.IsSatisfiedBy(candidate);

    /// <summary>
    /// Determines whether multiple candidate objects satisfy a specification.
    /// </summary>
    /// <typeparam name="T">The type of the candidate objects.</typeparam>
    /// <param name="candidates">The values to check.</param>
    /// <param name="spec">The specification to check.</param>
    /// <returns>
    /// <see langword="true"/> if all candidate objects satisfy the specification;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static UnitResult<IError> Are<T>(this IEnumerable<T> candidates, ISpecification<T> spec)
    {
        var results = candidates.Select(c => spec.IsSatisfiedBy(c));
        
        if(!results.Any(r => r.IsFailure))
            return UnitResult.Success<IError>();

        return new Errors(results.Select(r => r.Error).ToArray());
    }

    /// <summary>
    /// Creates a specification that negates a specification.
    /// </summary>
    /// <typeparam name="T">The type of the candidate object.</typeparam>
    /// <param name="spec">The specification to negate.</param>
    /// <returns>
    /// An <see cref="INotSpecification{T}"/> representing the negation of the
    /// original specification.
    /// </returns>
    public static INotSpecification<T> Not<T>(this ISpecification<T> spec)
        => new NotSpecification<T>(spec);

    /// <summary>
    /// Creates a specification that combines two specifications with
    /// a conditional AND operator.
    /// </summary>
    /// <typeparam name="T">The type of the candidate object.</typeparam>
    /// <param name="left">The first specification to combine.</param>
    /// <param name="right">The second specification to combine.</param>
    /// <returns>
    /// An <see cref="IAndSpecification{T}"/> representing the combination of
    /// both specifications with a conditional AND operator.
    /// </returns>
    public static IAndSpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        => new AndSpecification<T>(left, right);

    /// <summary>
    /// Creates a specification that combines two specifications with
    /// a conditional OR operator.
    /// </summary>
    /// <typeparam name="T">The type of the candidate object.</typeparam>
    /// <param name="left">The first specification to combine.</param>
    /// <param name="right">The second specification to combine.</param>
    /// <returns>
    /// An <see cref="IOrSpecification{T}"/> representing the combination of
    /// both specifications with a conditional OR operator.
    /// </returns>
    public static IOrSpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        => new OrSpecification<T>(left, right);
}

file sealed class NotSpecification<T> : INotSpecification<T>
{
    public NotSpecification(ISpecification<T> operand)
    {
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    public ISpecification<T> Operand { get; }

    public UnitResult<IError> IsSatisfiedBy(T candidate)
        => !Operand.IsSatisfiedBy(candidate).IsSuccess ? UnitResult.Success<IError>() : UnitResult.Failure<IError>(Error.Null);

    public UnitResult<IError> IsSatisfiedBy(object candidate)
        => !Operand.IsSatisfiedBy(candidate).IsSuccess ? UnitResult.Success<IError>() : UnitResult.Failure<IError>(Error.Null);
}

file sealed class AndSpecification<T> : IAndSpecification<T>
{
    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public ISpecification<T> Left { get; }

    public ISpecification<T> Right { get; }

    public UnitResult<IError> IsSatisfiedBy(T candidate)
    {
        var l = Left.IsSatisfiedBy(candidate);

        if(l.IsFailure)
            return l;

        var r = Right.IsSatisfiedBy(candidate);

        if(r.IsFailure)
            return r;

        return UnitResult.Success<IError>();
    }

    public UnitResult<IError> IsSatisfiedBy(object candidate) => IsSatisfiedBy((T)candidate);
}

file sealed class OrSpecification<T> : IOrSpecification<T>
{
    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public ISpecification<T> Left { get; }

    public ISpecification<T> Right { get; }

    public UnitResult<IError> IsSatisfiedBy(T candidate)
    {
        var l = Left.IsSatisfiedBy(candidate);

        if(l.IsSuccess)
            return l;

        var r = Right.IsSatisfiedBy(candidate);

        if(r.IsSuccess)
            return r;

        return UnitResult.Failure<IError>(new Errors(l.Error, r.Error));
    }

    public UnitResult<IError> IsSatisfiedBy(object candidate) => IsSatisfiedBy((T)candidate);
}
