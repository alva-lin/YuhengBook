using YuhengBook.Core.BookAggregate;
using YuhengBook.UnitTests.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.UnitTests.UseCases.BookAggregate.Chapters;

public class DeleteChapterCommand_Tests : BasicTest
{
    private readonly IRepository<Chapter> _repos;
    private readonly DeleteChapterHandler _handler;

    public DeleteChapterCommand_Tests()
    {
        _repos = Substitute.For<IRepository<Chapter>>();
        _handler = new(_repos);
    }

    private static DeleteChapterCommand CreateCommand(long? id = null)
    {
        return new Faker<DeleteChapterCommand>()
           .CustomInstantiator(f => new(
                id ?? f.IndexGlobal
            )).Generate();
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandle_ThenReturnSuccess()
    {
        var cmd = CreateCommand();

        var mockChapter = Chapter_Tests.CreateInstance();
        _repos.SingleOrDefaultAsync(Arg.Any<SingleChapterSpec>(), Arg.Any<CancellationToken>())
           .Returns(mockChapter);

        var result = await _handler.Handle(cmd, default);

        result.IsSuccess.Should().BeTrue();

        await _repos.Received(1).DeleteAsync(Arg.Any<Chapter>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenInvalidCommand_WhenHandle_ThenReturnNotFound()
    {
        var cmd = CreateCommand();

        _repos.SingleOrDefaultAsync(Arg.Any<SingleChapterSpec>(), Arg.Any<CancellationToken>())
           .Returns((Chapter?)null);

        var result = await _handler.Handle(cmd, default);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        await _repos.DidNotReceive().DeleteAsync(Arg.Any<Chapter>(), Arg.Any<CancellationToken>());
    }
}
