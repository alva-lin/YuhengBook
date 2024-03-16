namespace YuhengBook.Core.BookAggregate;

public sealed class BookAddNewChapterEvent(long bookId, string title) : DomainEventBase
{
    public long BookId { get; } = bookId;

    public string Title { get; } = title;
}
