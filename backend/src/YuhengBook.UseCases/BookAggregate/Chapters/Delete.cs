using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record DeleteChapterCommand(long Id) : ICommand<Result>;

public class DeleteChapterHandler(IRepository<Chapter> repos) : ICommandHandler<DeleteChapterCommand, Result>
{
    public async Task<Result> Handle(DeleteChapterCommand req, CancellationToken ct)
    {
        var chapter = await repos.SingleOrDefaultAsync(new SingleChapterSpec(req.Id), ct);
        if (chapter is null)
        {
            return Result.NotFound().WithError(nameof(req.Id), "Chapter not found");
        }

        await repos.DeleteAsync(chapter, ct);

        return Result.Success();
    }
}
