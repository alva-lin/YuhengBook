using YuhengBook.Api.BookAggregate;

namespace YuhengBook.FunctionalTests.BookAggregate.Books;

public class CreateBook_Tests(AppFixture app) : TestBase<AppFixture>
{
    private static Faker<CreateBookRequest> FakeRequest()
    {
        return new Faker<CreateBookRequest>()
               .StrictMode(true)
               .RuleFor(x => x.Name, f => f.Random.String(1, DataSchemaConstants.DEFAULT_NAME_LENGTH))
               .RuleFor(x => x.Description,
                    f => f.Random.String(1, DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH).OrNull(f)
                )
            ;
    }

    public async static Task<long> CreateBook(AppFixture app, CreateBookRequest? request = null)
    {
        request ??= FakeRequest().Generate();

        var (resp, res) = await app.Client
           .POSTAsync<CreateBook, CreateBookRequest, Result<long>>(request);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        return res.Value;
    }

    [Fact]
    public async Task GivenValidModel_ReturnsSuccess()
    {
        var request = FakeRequest().Generate();

        var (resp, res) = await app.Client
           .POSTAsync<CreateBook, CreateBookRequest, Result<long>>(request);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GivenInvalidModel_ReturnsBadRequest()
    {
        var baseRequest = FakeRequest().Generate();

        await Call_And_ExpectedBadRequest(
            baseRequest with { Name = null! },
            HttpStatusCode.BadRequest,
            nameof(CreateBookRequest.Name)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Name = string.Empty },
            HttpStatusCode.BadRequest,
            nameof(CreateBookRequest.Name)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Name = Fake.Random.String(DataSchemaConstants.DEFAULT_NAME_LENGTH + 1) },
            HttpStatusCode.BadRequest,
            nameof(CreateBookRequest.Name)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Description = Fake.Random.String(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH + 1) },
            HttpStatusCode.BadRequest,
            nameof(CreateBookRequest.Description)
        );
    }

    private async Task Call_And_ExpectedBadRequest(
        CreateBookRequest request,
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest,
        string? expectedErrorField = null
    )
    {
        var (resp, res) = await app.Client
           .POSTAsync<CreateBook, CreateBookRequest, ProblemDetails>(request);

        resp.StatusCode.Should().Be(expectedStatusCode);

        if (string.IsNullOrWhiteSpace(expectedErrorField))
        {
            res.Errors.Should().Contain(e =>
                e.Name.Equals(expectedErrorField, StringComparison.CurrentCultureIgnoreCase)
            );
        }
    }
}
