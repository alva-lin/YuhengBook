using YuhengBook.Api.BookAggregate;

namespace YuhengBook.FunctionalTests.BookAggregate.Books;

public class UpdateBook_Tests(AppFixture app) : TestBase<AppFixture>
{
    private static Faker<UpdateBookRequest> FakeRequest(long id)
    {
        return new Faker<UpdateBookRequest>()
               .StrictMode(true)
               .RuleFor(x => x.Id, _ => id)
               .RuleFor(x => x.Name, f => f.Random.String(1, DataSchemaConstants.DEFAULT_NAME_LENGTH))
               .RuleFor(x => x.Description,
                    f => f.Random.String(1, DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH).OrNull(f)
                )
            ;
    }

    [Fact]
    public async Task GivenValidModel_ReturnsSuccess()
    {
        var id = await CreateBook_Tests.CreateBook(app);

        var request = FakeRequest(id).Generate();

        var resp = await app.Client
           .PUTAsync<UpdateBook, UpdateBookRequest>(request);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GivenInvalidModel_ReturnsBadRequest()
    {
        var id = await CreateBook_Tests.CreateBook(app);

        var baseRequest = FakeRequest(id).Generate();

        await Call_And_ExpectedBadRequest(
            baseRequest with { Name = null! },
            HttpStatusCode.BadRequest,
            nameof(UpdateBookRequest.Name)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Name = string.Empty },
            HttpStatusCode.BadRequest,
            nameof(UpdateBookRequest.Name)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Name = Fake.Random.String(DataSchemaConstants.DEFAULT_NAME_LENGTH + 1) },
            HttpStatusCode.BadRequest,
            nameof(UpdateBookRequest.Name)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Description = Fake.Random.String(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH + 1) },
            HttpStatusCode.BadRequest,
            nameof(UpdateBookRequest.Description)
        );
    }

    private async Task Call_And_ExpectedBadRequest(
        UpdateBookRequest request,
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest,
        string? expectedErrorField = null
    )
    {
        var (resp, res) = await app.Client
           .PUTAsync<UpdateBook, UpdateBookRequest, ProblemDetails>(request);

        resp.StatusCode.Should().Be(expectedStatusCode);

        if (string.IsNullOrWhiteSpace(expectedErrorField))
        {
            res.Errors.Should().Contain(e =>
                e.Name.Equals(expectedErrorField, StringComparison.CurrentCultureIgnoreCase)
            );
        }
    }
}
