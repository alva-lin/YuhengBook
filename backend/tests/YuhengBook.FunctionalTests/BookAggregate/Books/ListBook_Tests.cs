using YuhengBook.Api.BookAggregate;
using YuhengBook.Core.BookAggregate;
using YuhengBook.UseCases;

namespace YuhengBook.FunctionalTests.BookAggregate.Books;

public class ListBook_Tests(AppFixture app) : TestBase<AppFixture>
{
    [Fact]
    public async Task GivenValidModel_ReturnsSuccess()
    {
        var id = await CreateBook_Tests.CreateBook(app);

        var request = new Faker<ListBookRequest>()
           .StrictMode(true)
           .RuleFor(x => x.PageSize, _ => 10)
           .RuleFor(x => x.Page, _ => 1)
           .RuleFor(x => x.Keyword, _ => null)
           .Generate();
        var (resp, res) = await app.Client
           .GETAsync<ListBook, ListBookRequest, Result<PaginatedList<BookInfoDto>>>(request);

        var a = await resp.Content.ReadAsStringAsync();
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Value.Data.Should().NotBeEmpty();
        res.Value.Data.Should().Contain(x => x.Id == id);
    }

    [Fact]
    public async Task GivenInvalidModel_ReturnsBadRequest()
    {
        await CreateBook_Tests.CreateBook(app);

        var baseRequest = new Faker<ListBookRequest>()
           .StrictMode(true)
           .RuleFor(x => x.PageSize, _ => 10)
           .RuleFor(x => x.Page, _ => 1)
           .RuleFor(x => x.Keyword, _ => null)
           .Generate();

        await Call_And_ExpectedBadRequest(
            baseRequest with { Page = -1 },
            HttpStatusCode.BadRequest,
            nameof(ListBookRequest.Page)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { PageSize = 0 },
            HttpStatusCode.BadRequest,
            nameof(ListBookRequest.PageSize)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { PageSize = DataSchemaConstants.PAGE_MAX_SIZE + 1 },
            HttpStatusCode.BadRequest,
            nameof(ListBookRequest.PageSize)
        );

        await Call_And_ExpectedBadRequest(
            baseRequest with { Keyword = Fake.Random.String(DataSchemaConstants.DEFAULT_NAME_LENGTH + 1) },
            HttpStatusCode.BadRequest,
            nameof(ListBookRequest.Keyword)
        );
    }

    private async Task Call_And_ExpectedBadRequest(
        ListBookRequest request,
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest,
        string? expectedErrorField = null
    )
    {
        var (resp, res) = await app.Client
           .GETAsync<ListBook, ListBookRequest, ProblemDetails>(request);

        resp.StatusCode.Should().Be(expectedStatusCode);

        if (string.IsNullOrWhiteSpace(expectedErrorField))
        {
            res.Errors.Should().Contain(e =>
                e.Name.Equals(expectedErrorField, StringComparison.CurrentCultureIgnoreCase)
            );
        }
    }
}
