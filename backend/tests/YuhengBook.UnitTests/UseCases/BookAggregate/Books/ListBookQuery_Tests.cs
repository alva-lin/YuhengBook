using YuhengBook.Core.BookAggregate;
using YuhengBook.UnitTests.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.UnitTests.UseCases.BookAggregate.Books;

public class ListBookQuery_Tests : BasicTest
{
    private readonly IReadRepository<Book> _repos;
    private readonly ListBookQueryHandler  _handler;

    public ListBookQuery_Tests()
    {
        _repos = Substitute.For<IReadRepository<Book>>();
        _handler = new(_repos);
    }

    public static ListBookQuery CreateQuery(string? keyword = null, int pageSize = 10, int page = 1)
    {
        return new Faker<ListBookQuery>()
               .CustomInstantiator(f => new ListBookQuery(
                    pageSize, page, keyword
                ))
               .Generate()
            ;
    }

    [Fact]
    public async Task GivenValidQuery_WhenHandle_ThenReturnBooks()
    {
        var query = CreateQuery();

        var mockBooks = Book_Tests.CreateList(query.PageSize);
        var dtos      = mockBooks.Select(BookInfoDto.FromEntity).ToList();

        var mockCount = Fake.Random.Int(mockBooks.Count);

        _repos.ListAsync(Arg.Any<BookSpec>(), Arg.Any<CancellationToken>())
           .Returns(dtos);
        _repos.CountAsync(Arg.Any<BookSpec>(), Arg.Any<CancellationToken>())
           .Returns(mockCount);

        var result = await _handler.Handle(query, default);

        var paginatedList = result.Value;
        paginatedList.Data.Should().BeEquivalentTo(dtos);
        paginatedList.TotalCount.Should().Be(mockCount);
        paginatedList.Page.Should().Be(query.Page);
        paginatedList.PageSize.Should().Be(query.PageSize);
    }
}
