using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record UpdateChapterCommand(long Id, string Title) : ICommand<Result>;

public class UpdateChapterHandler(IRepository<Chapter> repos) : ICommandHandler<UpdateChapterCommand, Result>
{
    public async Task<Result> Handle(UpdateChapterCommand req, CancellationToken ct)
    {
        var chapter = await repos.SingleOrDefaultAsync(new SingleChapterSpec(req.Id), ct);
        if (chapter is null)
        {
            return Result.NotFound().WithError(nameof(req.Id), "Chapter not found");
        }

        chapter.Title = req.Title;

        await repos.UpdateAsync(chapter, ct);

        return Result.Success();
    }
}
