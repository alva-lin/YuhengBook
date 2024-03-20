using YuhengBook.Api.BookAggregate.Chapters;
using YuhengBook.FunctionalTests.BookAggregate.Books;

namespace YuhengBook.FunctionalTests.BookAggregate.Chapters;

public class CreateChapter_Tests(AppFixture app) : TestBase<AppFixture>
{
    private static Faker<CreateChapterRequest> FakeRequest(long bookId)
    {
        return new Faker<CreateChapterRequest>()
               .StrictMode(true)
               .RuleFor(x => x.BookId, _ => bookId)
               .RuleFor(x => x.Order, _ => null)
               .RuleFor(x => x.Title, f => f.Random.String(1, DataSchemaConstants.DEFAULT_NAME_LENGTH))
               .RuleFor(x => x.Content,
                    f => f.Random.String(1, DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH)
                )
            ;
    }

    public async static Task<long> CreateChapter(AppFixture app, CreateChapterRequest? request = null)
    {
        var bookId = await CreateBook_Tests.CreateBook(app);
        request ??= FakeRequest(bookId).Generate();

        var (resp, res) = await app.Client
           .POSTAsync<CreateChapter, CreateChapterRequest, long>(request);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        return res;
    }

    [Fact]
    public async Task GivenValidModel_ReturnsSuccess()
    {
        var bookId = await CreateBook_Tests.CreateBook(app);

        var request = FakeRequest(bookId).Generate();

        var (resp, _) = await app.Client
           .POSTAsync<CreateChapter, CreateChapterRequest, long>(request);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GivenInvalidModel_ReturnsBadRequest()
    {
        var bookId = await CreateBook_Tests.CreateBook(app);

        var baseRequest = FakeRequest(bookId).Generate();

        await Call_And_ExpectedBadRequest(
            baseRequest with { Title = null! },
            HttpStatusCode.BadRequest,
            nameof(CreateChapterRequest.Title)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Title = string.Empty },
            HttpStatusCode.BadRequest,
            nameof(CreateChapterRequest.Title)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Title = Fake.Random.String(DataSchemaConstants.DEFAULT_NAME_LENGTH + 1) },
            HttpStatusCode.BadRequest,
            nameof(CreateChapterRequest.Title)
        );
    }

    private async Task Call_And_ExpectedBadRequest(
        CreateChapterRequest request,
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest,
        string? expectedErrorField = null
    )
    {
        var (resp, res) = await app.Client
           .POSTAsync<CreateChapter, CreateChapterRequest, ProblemDetails>(request);

        resp.StatusCode.Should().Be(expectedStatusCode);

        if (string.IsNullOrWhiteSpace(expectedErrorField))
        {
            res.Errors.Should().Contain(e =>
                e.Name.Equals(expectedErrorField, StringComparison.CurrentCultureIgnoreCase)
            );
        }
    }
}
