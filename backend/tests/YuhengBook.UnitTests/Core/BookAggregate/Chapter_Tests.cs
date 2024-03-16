using YuhengBook.Core.BookAggregate;

namespace YuhengBook.UnitTests.Core.BookAggregate;

public class Chapter_Tests : BasicTest
{
    public static Chapter CreateInstance(int? order = null)
    {
        return new Faker<Chapter>()
           .CustomInstantiator(f => new Chapter(
                order ?? f.IndexGlobal,
                f.Lorem.Word()
            ))
           .RuleFor(b => b.Detail, f => new ChapterDetail
            {
                Content = f.Lorem.Paragraph()
            })
           .Generate();
    }

    [Fact]
    public void Chapter_Constructor()
    {
        var order   = Fake.Random.Int(1, 100);
        var title   = Fake.Lorem.Word();
        var content = Fake.Lorem.Paragraph();

        var chapter = new Chapter(order, title)
        {
            Detail = new()
            {
                Content = content
            }
        };

        chapter.Order.Should().Be(order);
        chapter.Title.Should().Be(title);
        chapter.Content.Should().Be(content);
    }
}
