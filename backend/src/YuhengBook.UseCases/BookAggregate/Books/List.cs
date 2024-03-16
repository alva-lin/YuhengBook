using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record ListBookQuery(int PageSize, int Page, string? Keyword) : IQuery<Result<PaginatedList<BookInfoDto>>>;

public class ListBookQueryHandler(IReadRepository<Book> repos)
    : IQueryHandler<ListBookQuery, Result<PaginatedList<BookInfoDto>>>
{
    public async Task<Result<PaginatedList<BookInfoDto>>> Handle(ListBookQuery req, CancellationToken ct)
    {
        var books = await repos.ListAsync(new BookSpec(req.Keyword, req.PageSize, req.Page), ct);
        var count = await repos.CountAsync(new BookSpec(req.Keyword), ct);

        var result = new PaginatedList<BookInfoDto>(books, count, req.Page, req.PageSize);
        return result;
    }
}
