using YuhengBook.Core.BookAggregate;
using YuhengBook.UnitTests.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.UnitTests.UseCases.BookAggregate.Books;

public class GetBookQuery_Tests : BasicTest
{
    private readonly IReadRepository<Book> _repos;
    private readonly GetBookQueryHandler   _handler;

    public GetBookQuery_Tests()
    {
        _repos = Substitute.For<IReadRepository<Book>>();
        _handler = new(_repos);
    }

    public static GetBookQuery CreateQuery(long? id = null)
    {
        return new Faker<GetBookQuery>()
               .CustomInstantiator(f => new(id ?? f.IndexGlobal))
               .Generate()
            ;
    }

    [Fact]
    public async Task GivenValidQuery_WhenHandle_ThenReturnBook()
    {
        var query = CreateQuery();

        var mockBook = Book_Tests.CreateInstance();
        var dto = BookDetailDto.FromEntity(mockBook);

        _repos.SingleOrDefaultAsync(Arg.Any<SingleBookSpec>(), Arg.Any<CancellationToken>())
           .Returns(mockBook);

        var result = await _handler.Handle(query, default);

        result.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GivenInvalidQuery_WhenHandle_ThenReturnNotFound()
    {
        var query = CreateQuery();

        _repos.SingleOrDefaultAsync(Arg.Any<SingleBookSpec>(), Arg.Any<CancellationToken>())
           .Returns((Book?)null);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }
}
