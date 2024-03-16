namespace YuhengBook.Core.BookAggregate;

public class Book : EntityBase<long>, IAggregateRoot
{
    public string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    public string? Description { get; set; }

    public IReadOnlyList<Chapter> Chapters => _chapters.AsReadOnly();

    private readonly List<Chapter> _chapters = [];
    private          string        _name     = null!;

    public Chapter AddChapter(string title, string content, int? order)
    {
        if (order < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(order), "Chapter order must be greater than 0.");
        }

        if (_chapters.Any(c => c.Order == order))
        {
            throw new InvalidOperationException($"Chapter with order {order} already exists.");
        }

        var chapter = new Chapter(order ?? _chapters.Count + 1, title)
        {
            Book = this,
            BookId = Id,
            Detail = new()
            {
                Content = content
            }
        };

        _chapters.Add(chapter);

        _chapters.Sort((x, y) => x.Order.CompareTo(y.Order));

        // TODO - 处理 BookAddNewChapterEvent

        return chapter;
    }
}
