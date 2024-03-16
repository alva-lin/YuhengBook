using YuhengBook.Api.BookAggregate.Chapters;

namespace YuhengBook.FunctionalTests.BookAggregate.Chapters;

public class UpdateChapter_Tests(AppFixture app) : TestBase<AppFixture>
{
    private static Faker<UpdateChapterRequest> FakeRequest(long id)
    {
        return new Faker<UpdateChapterRequest>()
               .StrictMode(true)
               .RuleFor(x => x.Id, _ => id)
               .RuleFor(x => x.Title, f => f.Random.String(1, DataSchemaConstants.DEFAULT_NAME_LENGTH))
            ;
    }

    [Fact]
    public async Task GivenValidModel_ReturnsSuccess()
    {
        var chapterId = await CreateChapter_Tests.CreateChapter(app);

        var request = FakeRequest(chapterId).Generate();

        var resp = await app.Client
           .PUTAsync<UpdateChapter, UpdateChapterRequest>(request);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GivenInvalidModel_ReturnsBadRequest()
    {
        var chapterId = await CreateChapter_Tests.CreateChapter(app);

        var baseRequest = FakeRequest(chapterId).Generate();

        await Call_And_ExpectedBadRequest(
            baseRequest with { Title = null! },
            HttpStatusCode.BadRequest,
            nameof(UpdateChapterRequest.Title)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Title = string.Empty },
            HttpStatusCode.BadRequest,
            nameof(UpdateChapterRequest.Title)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Title = Fake.Random.String(DataSchemaConstants.DEFAULT_NAME_LENGTH + 1) },
            HttpStatusCode.BadRequest,
            nameof(UpdateChapterRequest.Title)
        );
    }

    private async Task Call_And_ExpectedBadRequest(
        UpdateChapterRequest request,
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest,
        string? expectedErrorField = null
    )
    {
        var (resp, res) = await app.Client
           .PUTAsync<UpdateChapter, UpdateChapterRequest, ProblemDetails>(request);

        resp.StatusCode.Should().Be(expectedStatusCode);

        if (string.IsNullOrWhiteSpace(expectedErrorField))
        {
            res.Errors.Should().Contain(e =>
                e.Name.Equals(expectedErrorField, StringComparison.CurrentCultureIgnoreCase)
            );
        }
    }
}
