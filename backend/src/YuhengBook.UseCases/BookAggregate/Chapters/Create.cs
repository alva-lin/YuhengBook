using YuhengBook.Core.Attributes;
using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record CreateChapterCommand(long BookId, string Title,[property: LogIgnore] string Content, int? Order) : ICommand<Result<long>>;

public class CreateChapterHandler(IRepository<Book> repos, IRepository<Chapter> chapterRepos) : ICommandHandler<CreateChapterCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateChapterCommand req, CancellationToken cancellationToken)
    {
        var book = await repos.SingleOrDefaultAsync(new SingleBookSpec(req.BookId, includeChapters: true),
            cancellationToken);
        if (book is null)
        {
            return Result.NotFound().WithError(
                nameof(req.BookId),
                "Book not found"
            );
        }

        var chapter = book.AddChapter(req.Title, req.Content, req.Order);

        await chapterRepos.AddAsync(chapter, cancellationToken);

        return chapter.Id;
    }
}
