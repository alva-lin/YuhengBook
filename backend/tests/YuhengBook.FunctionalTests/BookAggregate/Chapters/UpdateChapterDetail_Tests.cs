using YuhengBook.Api.BookAggregate.Chapters;

namespace YuhengBook.FunctionalTests.BookAggregate.Chapters;

public class UpdateChapterDetail_Tests(AppFixture app) : TestBase<AppFixture>
{
    private static Faker<UpdateChapterDetailRequest> FakeRequest(long id)
    {
        return new Faker<UpdateChapterDetailRequest>()
               .StrictMode(true)
               .RuleFor(x => x.Id, _ => id)
               .RuleFor(x => x.Content, f => f.Lorem.Paragraphs())
            ;
    }

    [Fact]
    public async Task GivenValidModel_ReturnsSuccess()
    {
        var chapterId = await CreateChapter_Tests.CreateChapter(app);

        var request = FakeRequest(chapterId).Generate();

        var resp = await app.Client
           .PUTAsync<UpdateChapterDetail, UpdateChapterDetailRequest>(request);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
