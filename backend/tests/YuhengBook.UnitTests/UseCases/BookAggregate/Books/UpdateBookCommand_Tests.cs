using YuhengBook.Core.BookAggregate;
using YuhengBook.UnitTests.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.UnitTests.UseCases.BookAggregate.Books;

public class UpdateBookCommand_Tests : BasicTest
{
    private readonly IRepository<Book> _repos;
    private readonly UpdateBookHandler _handler;

    public UpdateBookCommand_Tests()
    {
        _repos = Substitute.For<IRepository<Book>>();
        _handler = new(_repos);
    }

    public static UpdateBookCommand CreateCommand(long? id = null)
    {
        return new Faker<UpdateBookCommand>()
               .CustomInstantiator(f => new(
                    id ?? f.Random.Long(),
                    f.Lorem.Word(),
                    f.Lorem.Sentence().OrNull(f)
                ))
               .Generate()
            ;
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandle_ThenReturnId()
    {
        var cmd = CreateCommand();

        var mockBook = Book_Tests.CreateInstance(cmd.Id);

        _repos.SingleOrDefaultAsync(Arg.Any<SingleBookSpec>(), Arg.Any<CancellationToken>())
           .Returns(mockBook);

        var result = await _handler.Handle(cmd, default);

        result.IsSuccess.Should().BeTrue();

        await _repos.Received(1).UpdateAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenInvalidCommand_WhenHandle_ThenThrowException()
    {
        var cmd = CreateCommand();

        _repos.SingleOrDefaultAsync(Arg.Any<SingleBookSpec>(), Arg.Any<CancellationToken>())
           .Returns((Book?)null);

        var result = await _handler.Handle(cmd, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        await _repos.DidNotReceive().UpdateAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
    }
}
