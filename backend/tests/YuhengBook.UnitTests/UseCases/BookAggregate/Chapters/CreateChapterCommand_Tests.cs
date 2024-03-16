using YuhengBook.Core.BookAggregate;
using YuhengBook.UnitTests.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.UnitTests.UseCases.BookAggregate.Chapters;

public class CreateChapterCommand_Tests : BasicTest
{
    private readonly IRepository<Book>    _repos;
    private readonly CreateChapterHandler _handler;

    public CreateChapterCommand_Tests()
    {
        _repos = Substitute.For<IRepository<Book>>();
        _handler = new(_repos);
    }

    private static CreateChapterCommand CreateCommand(long? bookId = null, int? order = null)
    {
        return new Faker<CreateChapterCommand>()
           .CustomInstantiator(f => new(
                bookId ?? f.IndexGlobal,
                f.Lorem.Word(),
                f.Lorem.Paragraph(),
                order ?? 1
            )).Generate();
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandle_ThenReturnSuccess()
    {
        var cmd = CreateCommand();

        var mockBook = Book_Tests.CreateInstance(cmd.BookId);
        _repos.SingleOrDefaultAsync(Arg.Any<SingleBookSpec>(), Arg.Any<CancellationToken>())
           .Returns(mockBook);

        var result = await _handler.Handle(cmd, default);

        result.IsSuccess.Should().BeTrue();

        mockBook.Chapters.Should().HaveCount(1);

        var chapter = mockBook.Chapters.First();
        chapter.Order.Should().Be(cmd.Order);
        chapter.Title.Should().Be(cmd.Title);
        chapter.Content.Should().Be(cmd.Content);
        chapter.BookId.Should().Be(cmd.BookId);

        await _repos.Received(1).UpdateAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
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

        await _repos.DidNotReceive().UpdateAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
    }
}
