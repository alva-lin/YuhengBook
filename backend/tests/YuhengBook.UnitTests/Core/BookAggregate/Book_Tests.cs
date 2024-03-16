using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UnitTests.Core.BookAggregate;

public class Book_Tests : BasicTest
{
    public static Book CreateInstance(long? id = null)
    {
        return new Faker<Book>()
           .CustomInstantiator(f => new Book())
           .RuleFor(b => b.Id, f => id ?? f.IndexGlobal)
           .RuleFor(b => b.Name, f => f.Lorem.Word())
           .RuleFor(b => b.Description, f => f.Lorem.Sentence().OrNull(f))
           .Generate();
    }

    public static List<Book> CreateList(int count = 3)
    {
        return new Faker<Book>()
           .CustomInstantiator(f => new Book())
           .RuleFor(b => b.Id, f => f.IndexGlobal)
           .RuleFor(b => b.Name, f => f.Lorem.Word())
           .RuleFor(b => b.Description, f => f.Lorem.Sentence().OrNull(f))
           .Generate(count);
    }

    [Fact]
    public void Constructor_Success()
    {
        var name        = Fake.Lorem.Word();
        var description = Fake.Lorem.Sentence().OrNull(Fake);

        var book = new Book
        {
            Name = name,
            Description = description
        };

        book.Name.Should().Be(name);
        book.Description.Should().Be(description);
        book.Chapters.Should().BeEmpty();
    }

    [Fact]
    public void AddChapter_Success()
    {
        var book = CreateInstance();

        book.AddChapter(Fake.Lorem.Word(), Fake.Lorem.Paragraph(), null);

        book.Chapters.Should().HaveCount(1);
        book.Chapters.First().Order.Should().Be(1);
    }

    [Fact]
    public void AddChapter_WithUnorderedChapters()
    {
        var book = CreateInstance();

        book.AddChapter(Fake.Lorem.Word(), Fake.Lorem.Paragraph(), null);
        book.AddChapter(Fake.Lorem.Word(), Fake.Lorem.Paragraph(), null);
        book.AddChapter(Fake.Lorem.Word(), Fake.Lorem.Paragraph(), null);

        book.Chapters.Should().HaveCount(3);
        book.Chapters.Select(c => c.Order).Should().BeInAscendingOrder();
    }

    [Fact]
    public void AddChapter_WithNegativeOrder()
    {
        var book = CreateInstance();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            book.AddChapter(Fake.Lorem.Word(), Fake.Lorem.Paragraph(), -1);
        });
    }

    [Fact]
    public void AddChapter_WithExistingOrder()
    {
        var book = CreateInstance();

        book.AddChapter(Fake.Lorem.Word(), Fake.Lorem.Paragraph(), 1);

        Assert.Throws<InvalidOperationException>(() =>
        {
            book.AddChapter(Fake.Lorem.Word(), Fake.Lorem.Paragraph(), 1);
        });
    }
}
