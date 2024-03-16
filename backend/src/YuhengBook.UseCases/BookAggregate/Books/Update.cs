using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record UpdateBookCommand(long Id, string Name, string? Description) : ICommand<Result>;

public class UpdateBookHandler(IRepository<Book> repos) : ICommandHandler<UpdateBookCommand, Result>
{
    public async Task<Result> Handle(UpdateBookCommand req, CancellationToken ct)
    {
        var book = await repos.SingleOrDefaultAsync(new SingleBookSpec(req.Id), ct);
        if (book is null)
        {
            return Result.NotFound().WithError(nameof(req.Id), "Book not found");
        }

        book.Name = req.Name;
        book.Description = req.Description;

        await repos.UpdateAsync(book, ct);

        return Result.Success();
    }
}
