using YuhengBook.Core.BookAggregate;
using YuhengBook.UnitTests.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.UnitTests.UseCases.BookAggregate.Chapters;

public class GetChapterQuery_Tests : BasicTest
{
    private readonly IReadRepository<Chapter> _repos;
    private readonly GetChapterQueryHandler   _handler;

    public GetChapterQuery_Tests()
    {
        _repos = Substitute.For<IReadRepository<Chapter>>();
        _handler = new(_repos);
    }

    public static GetChapterQuery CreateQuery(long? bookId = null, int? order = null)
    {
        return new Faker<GetChapterQuery>()
           .CustomInstantiator(f => new(
                bookId ?? f.IndexGlobal,
                order ?? f.IndexGlobal
            )).Generate();
    }

    [Fact]
    public async Task GivenValidQuery_WhenHandle_ThenReturnSuccess()
    {
        var query = CreateQuery();

        var mockChapter = Chapter_Tests.CreateInstance();
        _repos.SingleOrDefaultAsync(Arg.Any<SingleChapterSpec>(), Arg.Any<CancellationToken>())
           .Returns(mockChapter);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();

        result.Value.BookId.Should().Be(mockChapter.BookId);
        result.Value.Order.Should().Be(mockChapter.Order);
        result.Value.Title.Should().Be(mockChapter.Title);
        result.Value.Content.Should().Be(mockChapter.Content);
    }

    [Fact]
    public async Task GivenInvalidQuery_WhenHandle_ThenReturnNotFound()
    {
        var query = CreateQuery();

        _repos.SingleOrDefaultAsync(Arg.Any<SingleChapterSpec>(), Arg.Any<CancellationToken>())
           .Returns((Chapter?)null);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }
}
