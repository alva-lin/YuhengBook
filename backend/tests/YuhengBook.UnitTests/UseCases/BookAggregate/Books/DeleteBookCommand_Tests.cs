using YuhengBook.Core.BookAggregate;
using YuhengBook.UnitTests.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.UnitTests.UseCases.BookAggregate.Books;

public class DeleteBookCommand_Tests : BasicTest
{
    private readonly IRepository<Book> _repos;
    private readonly DeleteBookHandler _handler;

    public DeleteBookCommand_Tests()
    {
        _repos = Substitute.For<IRepository<Book>>();
        _handler = new(_repos);
    }

    public static DeleteBookCommand CreateCommand(long? id = null)
    {
        return new Faker<DeleteBookCommand>()
               .CustomInstantiator(f => new(id ?? f.IndexGlobal))
               .Generate()
            ;
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandle_ThenReturnId()
    {
        var cmd = CreateCommand();

        var mockBook = Book_Tests.CreateInstance();

        _repos.SingleOrDefaultAsync(Arg.Any<SingleBookSpec>(), Arg.Any<CancellationToken>())
           .Returns(mockBook);

        var result = await _handler.Handle(cmd, default);

        result.IsSuccess.Should().BeTrue();

        await _repos.Received(1).DeleteAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenInvalidCommand_WhenHandle_ThenReturnNotFound()
    {
        var cmd = CreateCommand();

        _repos.SingleOrDefaultAsync(Arg.Any<SingleBookSpec>(), Arg.Any<CancellationToken>())
           .Returns((Book?)null);

        var result = await _handler.Handle(cmd, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        await _repos.DidNotReceive().DeleteAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
    }
}
