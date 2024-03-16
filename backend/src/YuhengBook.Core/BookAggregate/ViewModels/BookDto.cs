namespace YuhengBook.Core.BookAggregate;

public record BookInfoDto(
    long Id,
    string Name,
    string? Description,
    int ChapterCount,
    ChapterInfoDto? LastChapter
)
{
    public static BookInfoDto FromEntity(Book book)
    {
        var lastChapter = book.Chapters.MaxBy(c => c.Order);
        return new BookInfoDto(
            book.Id,
            book.Name,
            book.Description,
            book.Chapters.Count,
            lastChapter is null
                ? null
                : ChapterInfoDto.FromEntity(lastChapter)
        );

    }
}

public record BookDetailDto(
    long Id,
    string Name,
    string? Description,
    int ChapterCount,
    ChapterInfoDto? LastChapter,
    List<ChapterInfoDto> Chapters
)
{
    public static BookDetailDto FromEntity(Book mockBook)
    {
        var lastChapter = mockBook.Chapters.MaxBy(c => c.Order);
        return new BookDetailDto(
            mockBook.Id,
            mockBook.Name,
            mockBook.Description,
            mockBook.Chapters.Count,
            lastChapter is null
                ? null
                : ChapterInfoDto.FromEntity(lastChapter),
            mockBook.Chapters.Select(ChapterInfoDto.FromEntity).ToList()
        );
    }
}

public record ChapterInfoDto(int Order, string Title)
{
    public static ChapterInfoDto FromEntity(Chapter chapter) => new(chapter.Order, chapter.Title);
}

public record ChapterDetailDto(long BookId, int Order, string Title, string Content);
