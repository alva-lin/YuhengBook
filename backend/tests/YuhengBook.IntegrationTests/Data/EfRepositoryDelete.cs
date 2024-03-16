using YuhengBook.Core.BookAggregate;

namespace YuhengBook.IntegrationTests.Data;

public class EfRepositoryDelete : BaseEfRepoTestFixture
{
    [Fact]
    public async Task DeletesItemAfterAddingIt()
    {
        // add a Book
        var repository      = GetRepository();
        var testTitle       = Fake.Lorem.Word();
        var testDescription = Fake.Lorem.Sentence().OrNull(Fake);
        var book = new Book()
        {
            Name = testTitle,
            Description = testDescription,
        };

        await repository.AddAsync(book);

        // delete the item
        await repository.DeleteAsync(book);

        // verify it's no longer there
        Assert.DoesNotContain(await repository.ListAsync(),
            book => book.Name == testTitle);
    }
}
