using YuhengBook.Api.BookAggregate;
using YuhengBook.Core.BookAggregate;

namespace YuhengBook.FunctionalTests.BookAggregate.Books;

public class GetBook_Tests(AppFixture app) : TestBase<AppFixture>
{
    private static Faker<GetBookRequest> FakerRequest(long id)
    {
        return new Faker<GetBookRequest>()
               .StrictMode(true)
               .RuleFor(x => x.Id, _ => id)
            ;
    }

    [Fact]
    public async Task GivenValidModel_ReturnsSuccess()
    {
        var id = await CreateBook_Tests.CreateBook(app);

        var request = FakerRequest(id).Generate();

        var (resp, res) = await app.Client
           .GETAsync<GetBook, GetBookRequest, BookDetailDto>(request);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Id.Should().Be(id);
    }
}
