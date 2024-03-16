using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record GetBookQuery(long Id) : IQuery<Result<BookDetailDto>>;

public class GetBookQueryHandler(IReadRepository<Book> repos) : IQueryHandler<GetBookQuery, Result<BookDetailDto>>
{
    public async Task<Result<BookDetailDto>> Handle(GetBookQuery req, CancellationToken ct)
    {
        var book = await repos.SingleOrDefaultAsync(new SingleBookSpec(req.Id, includeChapters: true), ct);
        if (book is null)
        {
            return Result.NotFound().WithError(nameof(req.Id), "Book not found");
        }

        var chapters = book.Chapters.Select(c => new ChapterInfoDto(c.Order, c.Title)).ToList();

        var result = new BookDetailDto(
            book.Id,
            book.Name,
            book.Description,
            book.Chapters.Count,
            chapters.OrderByDescending(c => c.Order)
               .Select(c => new ChapterInfoDto(c.Order, c.Title))
               .FirstOrDefault(),
            chapters
        );

        return result;
    }
}
