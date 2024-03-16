using YuhengBook.Core.BookAggregate;

namespace YuhengBook.IntegrationTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
    [Fact]
    public async Task AddsContributorAndSetsId()
    {
        var testTitle       = Fake.Lorem.Word();
        var testDescription = Fake.Lorem.Sentence().OrNull(Fake);
        var repository      = GetRepository();
        var book = new Book
        {
            Name = testTitle,
            Description = testDescription,
        };

        await repository.AddAsync(book);

        var newBook = (await repository.ListAsync())
            .FirstOrDefault();

        Assert.Equal(testTitle, newBook?.Name);
        Assert.Equal(testDescription, newBook?.Description);
        Assert.True(newBook?.Id > 0);
    }
}
