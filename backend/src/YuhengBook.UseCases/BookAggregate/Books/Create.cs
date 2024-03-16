using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UseCases.BookAggregate;

public record CreateBookCommand(string Name, string? Description) : ICommand<Result<long>>;

public class CreateBookHandler(IRepository<Book> repos) : ICommandHandler<CreateBookCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateBookCommand req, CancellationToken cancellationToken)
    {
        var book = new Book
        {
            Name = req.Name,
            Description = req.Description,
        };

        await repos.AddAsync(book, cancellationToken);

        return book.Id;
    }
}
