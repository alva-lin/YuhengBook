using Ardalis.Specification;

namespace YuhengBook.Core.BookAggregate;

public sealed class BookSpec : Specification<Book, BookInfoDto>
{
    public BookSpec(string? keyword = null, int pageSize = 10, int page = 1)
    {
        Query.Where(b => b.Name.Contains(keyword!), !string.IsNullOrWhiteSpace(keyword));

        Query.Select(b => new(
            b.Id,
            b.Name,
            b.Description,
            b.Chapters.Count,
            b.Chapters
               .OrderByDescending(c => c.Order)
               .Select(c => new ChapterInfoDto(c.Order, c.Title))
               .FirstOrDefault()
        ));

        var skip = pageSize * (page - 1);
        var take = pageSize;

        Query.Skip(skip).Take(take);

        // TODO - 按时间倒序
        Query.OrderBy(b => b.Id);
    }
}
