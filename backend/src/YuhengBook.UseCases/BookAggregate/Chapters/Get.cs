using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record GetChapterQuery(long BookId, int Order) : IQuery<Result<ChapterDetailDto>>;

public class GetChapterQueryHandler(IReadRepository<Chapter> repos)
    : IQueryHandler<GetChapterQuery, Result<ChapterDetailDto>>
{
    public async Task<Result<ChapterDetailDto>> Handle(GetChapterQuery req, CancellationToken ct)
    {
        var chapter = await repos.SingleOrDefaultAsync(
            new SingleChapterSpec(req.BookId, req.Order, includeDetail: true),
            ct
        );
        if (chapter is null)
        {
            return Result.NotFound().WithError(nameof(req.Order), "Chapter not found");
        }

        var result = new ChapterDetailDto(
            chapter.Id,
            chapter.BookId,
            chapter.Order,
            chapter.Title,
            chapter.Content
        );

        return Result.Success(result);
    }
}
