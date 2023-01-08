namespace Friendbook.Shared.Models;

public class ListResult<T>
{
    public bool HasNextPage { get; init; }

    public int TotalCount { get; init; }

    public IEnumerable<T> Items { get; init; }

    public ListResult(IEnumerable<T> items, bool hasNextPage = false)
    {
        Items = items;
        TotalCount = items?.Count() ?? 0;
        HasNextPage = hasNextPage;
    }

    public ListResult(IEnumerable<T> items, int totalCount, bool hasNextPage = false)
    {
        Items = items;
        TotalCount = totalCount;
        HasNextPage = hasNextPage;
    }

    public ListResult()
    {
    }
}
