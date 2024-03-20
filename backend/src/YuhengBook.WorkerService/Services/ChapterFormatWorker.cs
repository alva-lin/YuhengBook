using MediatR;
using Microsoft.Extensions.Options;
using YuhengBook.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.WorkerService.Services;

public class ChapterFormatWorker(
    ILogger<ChapterFormatWorker> logger,
    IServiceScopeFactory scopeFactory,
    IOptionsMonitor<ChapterFormatOption> optionsMonitor) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = logger.BeginScope("ChapterFormatWorker");
        logger.LogInformation("ChapterFormatWorker started");

        var bookIds = await GetBookIds(stoppingToken);
        logger.LogInformation("Total books: {TotalBooks}", bookIds.Length);

        foreach (var bookId in bookIds)
        {
            logger.LogInformation("Formatting book {BookId}", bookId);

            var maxChapterOrder = await GetBookChapterCount(bookId, stoppingToken);
            for (var order = 1; order <= maxChapterOrder; order++)
            {
                var chapter = await GetChapter(bookId, order, stoppingToken);
                if (chapter is not null)
                {
                    var option           = optionsMonitor.CurrentValue;
                    var formattedContent = ChapterContentExtensions.FormatContent(chapter.Content, option);
                    await UpdateChapterContent(chapter.Id, formattedContent, stoppingToken);
                }

                await Task.Delay(100, stoppingToken);
            }

            logger.LogInformation("Book {BookId} formatted", bookId);
            await Task.Delay(1000, stoppingToken);
        }

        logger.LogInformation("ChapterFormatWorker stopped");
    }

    private async Task<long[]> GetBookIds(CancellationToken stoppingToken)
    {
        using var scope    = scopeFactory.CreateScope();
        var       mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request  = new ListBookQuery(1, 1, null);
        var response = await mediator.Send(request, stoppingToken);
        return response.Value.Data.Select(b => b.Id).ToArray();
    }

    private async Task<long> GetBookChapterCount(long bookId, CancellationToken stoppingToken)
    {
        using var scope    = scopeFactory.CreateScope();
        var       mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request  = new GetBookQuery(bookId);
        var response = await mediator.Send(request, stoppingToken);
        return response.Value.LastChapter?.Order ?? 0;
    }

    private async Task<ChapterDetailDto?> GetChapter(long bookId, int order, CancellationToken cancellationToken)
    {
        using var scope    = scopeFactory.CreateScope();
        var       mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request  = new GetChapterQuery(bookId, order);
        var response = await mediator.Send(request, cancellationToken);
        if (response.IsSuccess)
        {
            return response.Value;
        }

        return null;
    }

    private async Task UpdateChapterContent(long chapterId, string content, CancellationToken cancellationToken)
    {
        using var scope    = scopeFactory.CreateScope();
        var       mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = new UpdateChapterDetailCommand(chapterId, content);
        await mediator.Send(request, cancellationToken);
    }
}
