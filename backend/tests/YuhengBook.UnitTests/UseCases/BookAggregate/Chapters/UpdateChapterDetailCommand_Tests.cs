﻿using YuhengBook.Core.BookAggregate;
using YuhengBook.UnitTests.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.UnitTests.UseCases.BookAggregate.Chapters;

public class UpdateChapterDetailCommand_Tests : BasicTest
{
    private readonly IRepository<Chapter>       _repos;
    private readonly IRepository<ChapterDetail> _detailRepos;
    private readonly UpdateChapterDetailHandler _handler;

    public UpdateChapterDetailCommand_Tests()
    {
        _repos = Substitute.For<IRepository<Chapter>>();
        _detailRepos = Substitute.For<IRepository<ChapterDetail>>();
        _handler = new(_repos, _detailRepos);
    }

    public static UpdateChapterDetailCommand CreateCommand(long? id = null)
    {
        return new Faker<UpdateChapterDetailCommand>()
           .CustomInstantiator(f => new(
                id ?? f.IndexGlobal,
                f.Lorem.Paragraph()
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
        mockChapter.Detail.Content.Should().Be(cmd.Content);

        await _detailRepos.Received(1).UpdateAsync(Arg.Any<ChapterDetail>(), Arg.Any<CancellationToken>());
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

        await _detailRepos.DidNotReceive().UpdateAsync(Arg.Any<ChapterDetail>(), Arg.Any<CancellationToken>());
    }
}
