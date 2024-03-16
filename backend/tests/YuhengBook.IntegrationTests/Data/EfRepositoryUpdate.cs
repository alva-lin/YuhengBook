using Microsoft.EntityFrameworkCore;
using YuhengBook.Core.BookAggregate;

namespace YuhengBook.IntegrationTests.Data;

public class EfRepositoryUpdate : BaseEfRepoTestFixture
{
    [Fact]
    public async Task UpdatesItemAfterAddingIt()
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

        // detach the item so we get a different instance
        DbContext.Entry(book).State = EntityState.Detached;

        // fetch the item and update its title
        var newBook = (await repository.ListAsync())
            .FirstOrDefault(b => b.Name == testTitle);
        if (newBook == null)
        {
            Assert.NotNull(newBook);
            return;
        }

        Assert.NotSame(book, newBook);
        var newTitle = Fake.Lorem.Word();
        newBook.Name = newTitle;

        // Update the item
        await repository.UpdateAsync(newBook);

        // Fetch the updated item
        var updatedItem = (await repository.ListAsync())
            .FirstOrDefault(b => b.Name == newTitle);

        Assert.NotNull(updatedItem);
        Assert.NotEqual(book.Name, updatedItem?.Name);
        Assert.Equal(book.Description, updatedItem?.Description);
        Assert.Equal(newBook.Id, updatedItem?.Id);
    }
}
