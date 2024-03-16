using YuhengBook.Api.BookAggregate;

namespace YuhengBook.FunctionalTests.BookAggregate.Books;

public class DeleteBook_Tests(AppFixture app) : TestBase<AppFixture>
{
    private static Faker<DeleteBookRequest> FakerRequest(long id)
    {
        return new Faker<DeleteBookRequest>()
               .StrictMode(true)
               .RuleFor(x => x.Id, _ => id)
            ;
    }

    [Fact]
    public async Task GivenValidModel_ReturnsSuccess()
    {
        var id = await CreateBook_Tests.CreateBook(app);

        var request = FakerRequest(id).Generate();

        var resp = await app.Client
           .DELETEAsync<DeleteBook, DeleteBookRequest>(request);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
