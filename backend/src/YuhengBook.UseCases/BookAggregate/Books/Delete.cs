using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record DeleteBookCommand(long Id) : ICommand<Result>;

public class DeleteBookHandler(IRepository<Book> repos) : ICommandHandler<DeleteBookCommand, Result>
{
    public async Task<Result> Handle(DeleteBookCommand req, CancellationToken ct)
    {
        var book = await repos.SingleOrDefaultAsync(new SingleBookSpec(req.Id), ct);
        if (book is null)
        {
            return Result.NotFound().WithError(nameof(req.Id), "Book not found");
        }

        await repos.DeleteAsync(book, ct);

        return Result.Success();
    }
}
