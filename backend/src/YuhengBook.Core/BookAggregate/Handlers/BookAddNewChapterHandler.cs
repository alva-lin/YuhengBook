using MediatR;
using Microsoft.Extensions.Logging;

namespace YuhengBook.Core.BookAggregate;

public class BookAddNewChapterHandler(ILogger<BookAddNewChapterHandler> logger)
    : INotificationHandler<BookAddNewChapterEvent>
{
    public async Task Handle(BookAddNewChapterEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        logger.LogInformation(
            "BookAddNewChapterEvent: {BookId}, {ChapterTitle}",
            notification.BookId,
            notification.Title
        );
    }
}
