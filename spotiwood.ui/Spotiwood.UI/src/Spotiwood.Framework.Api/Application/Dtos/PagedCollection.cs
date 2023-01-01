﻿namespace Spotiwood.Framework.Api.Application.Dtos;
public sealed class PagedCollection<T>
{
    public int Page { get; init; }

    public int Size { get; init; }

    public int Total { get; init; }

    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
}