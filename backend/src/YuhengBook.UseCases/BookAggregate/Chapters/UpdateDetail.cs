using YuhengBook.Core.Attributes;
using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record UpdateChapterDetailCommand(long Id,[property: LogIgnore] string Content) : ICommand<Result>;

public class UpdateChapterDetailHandler(IRepository<Chapter> repos, IRepository<ChapterDetail> detailRepos) : ICommandHandler<UpdateChapterDetailCommand, Result>
{
    public async Task<Result> Handle(UpdateChapterDetailCommand req, CancellationToken ct)
    {
        var chapterContent = await repos.SingleOrDefaultAsync(new SingleChapterSpec(req.Id, includeDetail: true), ct);
        if (chapterContent is null)
        {
            return Result.NotFound().WithError(nameof(req.Id), "Chapter not found");
        }

        var detail = chapterContent.Detail;
        detail.Content = req.Content;

        await detailRepos.UpdateAsync(detail, ct);

        return Result.Success();
    }
}
