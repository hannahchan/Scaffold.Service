namespace Scaffold.Application.Common.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

public class SortOrder<T> : IReadOnlyList<(LambdaExpression KeySelector, bool Descending)>
{
    private readonly IImmutableList<(LambdaExpression KeySelector, bool Descending)> sortOrder;

    private SortOrder(IImmutableList<(LambdaExpression KeySelector, bool Descending)> sortOrder)
    {
        this.sortOrder = sortOrder;
    }

    public int Count => this.sortOrder.Count;

    public (LambdaExpression KeySelector, bool Descending) this[int index]
    {
        get => this.sortOrder[index];
    }

    public static SortOrder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        where TKey : IComparable<TKey>
    {
        return new SortOrder<T>(ImmutableArray<(LambdaExpression KeySelector, bool Descending)>.Empty)
            .Add(keySelector, false);
    }

    public static SortOrder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        where TKey : IComparable<TKey>
    {
        return new SortOrder<T>(ImmutableArray<(LambdaExpression KeySelector, bool Descending)>.Empty)
            .Add(keySelector, true);
    }

    public SortOrder<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        where TKey : IComparable<TKey>
    {
        return this.Add(keySelector, false);
    }

    public SortOrder<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        where TKey : IComparable<TKey>
    {
        return this.Add(keySelector, true);
    }

    public IEnumerator<(LambdaExpression KeySelector, bool Descending)> GetEnumerator()
    {
        return this.sortOrder.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private SortOrder<T> Add(LambdaExpression keySelector, bool descending)
    {
        return new SortOrder<T>(this.sortOrder.Add((keySelector, descending)));
    }
}
