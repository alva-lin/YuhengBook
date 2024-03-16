using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record UpdateChapterDetailCommand(long Id, string Content) : ICommand<Result>;

public class UpdateChapterDetailHandler(IRepository<Chapter> repos) : ICommandHandler<UpdateChapterDetailCommand, Result>
{
    public async Task<Result> Handle(UpdateChapterDetailCommand req, CancellationToken ct)
    {
        var chapterContent = await repos.SingleOrDefaultAsync(new SingleChapterSpec(req.Id, includeDetail: true), ct);
        if (chapterContent is null)
        {
            return Result.NotFound().WithError(nameof(req.Id), "Chapter not found");
        }

        chapterContent.Detail.Content = req.Content;

        await repos.UpdateAsync(chapterContent, ct);

        return Result.Success();
    }
}
