namespace YuhengBook.UseCases;

public record PaginatedList<T>(IReadOnlyList<T> Data, long TotalCount, long Page, long PageSize)
{
    public long TotalPage => (long)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPage;

    public bool IsFirstPage => Page == 1;

    public bool IsLastPage => Page == TotalPage;

    PaginatedList(List<T> data) : this(data, data.Count, 1, data.Count) { }

    PaginatedList(List<T> data, long totalCount, long page, long pageSize) : this(
        data.AsReadOnly(),
        totalCount,
        page,
        pageSize
    )
    {
    }

    public static implicit operator PaginatedList<T>(PagedResult<List<T>> result) =>
        new(result.Value,
            result.PagedInfo.TotalRecords,
            result.PagedInfo.PageNumber,
            result.PagedInfo.PageSize
        );

    public static implicit operator PaginatedList<T>(Result<List<T>> result) => new(result.Value);
}
