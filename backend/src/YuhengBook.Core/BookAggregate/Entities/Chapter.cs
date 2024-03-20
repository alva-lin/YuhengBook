namespace YuhengBook.Core.BookAggregate;

public class Chapter(int order, string title) : EntityBase<long>, IAggregateRoot
{
    private int    _order = Guard.Against.NegativeOrZero(order);
    private string _title = Guard.Against.NullOrWhiteSpace(title);
    public  Book   Book { get; set; } = null!;

    public long BookId { get; set; }

    public ChapterDetail Detail { get; set; } = null!;

    public int Order
    {
        get => _order;
        set => _order = Guard.Against.NegativeOrZero(value);
    }

    public string Title
    {
        get => _title;
        set => _title = Guard.Against.NullOrWhiteSpace(value);
    }

    public string Content => Detail.Content;
}

public class ChapterDetail : EntityBase<long>, IAggregateRoot
{
    private string _content = null!;

    /// <summary>
    ///     可能没有内容，但是不允许为空
    /// </summary>
    public string Content
    {
        get => _content;
        set => _content = Guard.Against.Null(value);
    }
}
