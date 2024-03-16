using YuhengBook.Api.BookAggregate.Chapters;

namespace YuhengBook.FunctionalTests.BookAggregate.Chapters;

public class DeleteChapter_Tests(AppFixture app) : TestBase<AppFixture>
{
    [Fact]
    public async Task GivenValidModel_ReturnsSuccess()
    {
        var chapterId = await CreateChapter_Tests.CreateChapter(app);

        var resp = await app.Client
           .DELETEAsync<DeleteChapter, DeleteChapterRequest>(new() { Id = chapterId });

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}