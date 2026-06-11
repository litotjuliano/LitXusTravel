namespace LitXusTravel.Application.Common.Models;

public class PagedList<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static PagedList<T> Create(IEnumerable<T> source, int page, int pageSize, int totalCount)
        => new()
        {
            Items = source.ToList().AsReadOnly(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
}
