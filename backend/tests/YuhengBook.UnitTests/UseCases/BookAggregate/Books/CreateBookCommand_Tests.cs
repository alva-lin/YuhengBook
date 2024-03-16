using YuhengBook.Core.BookAggregate;
using YuhengBook.UnitTests.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.UnitTests.UseCases.BookAggregate.Books;

public class CreateBookCommand_Tests : BasicTest
{
    private readonly IRepository<Book> _repos;
    private readonly CreateBookHandler _handler;

    public CreateBookCommand_Tests()
    {
        _repos = Substitute.For<IRepository<Book>>();
        _handler = new(_repos);
    }

    public static CreateBookCommand CreateCommand()
    {
        return new Faker<CreateBookCommand>()
               .CustomInstantiator(f => new(
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

        var mockBook = Book_Tests.CreateInstance();

        _repos.AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>())
           .Returns(mockBook);
        _repos.When(x => x.AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>()))
           .Do(x => ((Book)x[0]).Id = mockBook.Id);

        var result = await _handler.Handle(cmd, default);

        result.Value.Should().Be(mockBook.Id);

        await _repos.Received(1).AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
    }
}
