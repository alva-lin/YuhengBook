using System.Net.Http.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.WorkerService.Services;

public class BqgNovelsWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<BqgNovelsWorker> logger,
    IOptionsMonitor<ChapterFormatOption> optionsMonitor,
    HttpClient client) : BackgroundService
{
    private const string BaseUrl = "https://www.bqgnovels.com/";

    private readonly List<BookInfo> _books = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope  = logger.BeginScope("BqgNovelsWorker");
        var       random = new Random();

        client.BaseAddress = new Uri(BaseUrl);

        logger.LogInformation("BqgNovelsWorker started - {Url}", BaseUrl);

        await CrawlBookList(stoppingToken);

        foreach (var book in _books)
        {
            var startOrder = 1;
            var (bookId, latestOrder) = await GetBookRecord(book.Title, stoppingToken);
            if (bookId > 0)
            {
                startOrder = latestOrder + 1;
            }
            else
            {
                bookId = await CreateBook(book.Title, book.Description, stoppingToken);
                if (bookId == 0)
                {
                    logger.LogWarning("Failed to create book. Title: {Title}", book.Title);
                    continue;
                }
            }

            if (startOrder > book.LatestChapterOrder)
            {
                continue;
            }


            for (var order = startOrder; order <= book.LatestChapterOrder; order++)
            {
                var chapter = await CrawlChapter(book.Id, order, stoppingToken);


                if (chapter is not null)
                {
                    chapter.Content = ChapterContentExtensions.FormatContent(
                        chapter.Content ?? string.Empty,
                        optionsMonitor.CurrentValue
                    );
                    await AddChapter(bookId, order, chapter.Title, chapter.Content, stoppingToken);
                }

                await Task.Delay(100, stoppingToken);
            }

            logger.LogInformation("Crawl book finished. Title: {Title}", book.Title);
            await Task.Delay(1000, stoppingToken);

        }

        logger.LogInformation("Crawl all books finished");
    }

    private async Task CrawlBookList(CancellationToken ct = default)
    {
        using var scope = logger.BeginScope("Crawl Book List");

        var path = "api/query/get_list";
        var param = new Dictionary<string, string?>
        {
            { "page", 1.ToString() },
            { "size", 200.ToString() },
        };

        var url = QueryHelpers.AddQueryString(path, param);

        try
        {
            logger.LogInformation("Get Book List...");
            var resp = await client.GetAsync(url, ct);
            if (resp.IsSuccessStatusCode)
            {
                var res = await resp.Content.ReadFromJsonAsync<ResponseModel<PaginatedList<BookInfo>>>(ct);
                if (res is not null)
                {
                    logger.LogInformation("Get Book List Success. Count: {Count}", res.Data.Count);
                    _books.AddRange(res.Data.Data);
                }
                else
                {
                    logger.LogWarning("Failed to get book list, response is null, url: {Url}", url);
                }
            }
            else
            {
                logger.LogWarning("Failed to get book list, response is not success status code [{Code}], url: {Url}",
                    resp.StatusCode, resp.RequestMessage?.RequestUri?.AbsoluteUri);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get book list");
        }
    }

    private async Task<ChapterInfo?> CrawlChapter(long bookId, int order, CancellationToken ct = default)
    {
        using var scope = logger.BeginScope("Crawl Chapter: {BookId}, {Order}", bookId, order);
        logger.LogInformation("Begin to get chapter");

        var path = "api/query/get_book_text";
        var param = new Dictionary<string, string?>
        {
            { "bookId", bookId.ToString() },
            { "id", order.ToString() },
        };

        var url = QueryHelpers.AddQueryString(path, param);

        try
        {
            var resp = await client.GetAsync(url, ct);
            if (resp.IsSuccessStatusCode)
            {
                var res = await resp.Content.ReadFromJsonAsync<ResponseModel<BookInfo>>(ct);
                if (res is not null)
                {
                    var chapter = res.Data.Chapters.FirstOrDefault(c => c.Order == order);
                    if (chapter is not null)
                    {
                        logger.LogInformation("Get chapter success, Title: {Title}", chapter.Title);
                        return chapter;
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get chapter");
        }

        return null;
    }

    private async Task<(long BookId, int LatestOrder)> GetBookRecord(string title, CancellationToken ct = default)
    {
        using var logScope = logger.BeginScope("Get Book: {Title}", title);
        using var scope    = scopeFactory.CreateScope();
        var       mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var query  = new ListBookQuery(10, 1, title);
        var result = await mediator.Send(query, ct);

        if (result.IsSuccess)
        {
            var book = result.Value.Data.FirstOrDefault(b => b.Name == title);
            if (book is not null)
            {
                logger.LogInformation("Get book success, {BookId}, {LatestChapterOrder}", book.Id,
                    book.LastChapter?.Order ?? 0);
                return (book.Id, book.LastChapter?.Order ?? 0);
            }

            logger.LogInformation("Book not found");
        }
        else
        {
            logger.LogError("Failed to get book record");
        }

        return (0, 0);
    }

    private async Task<long> CreateBook(string title, string content, CancellationToken ct = default)
    {
        using var logScope = logger.BeginScope("Create book: {Title}, text count: {TextCount}", title, content.Length);
        using var scope    = scopeFactory.CreateScope();
        var       mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var command = new CreateBookCommand(title, content);
        var result  = await mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            logger.LogError("Failed to create book");
            return 0;
        }

        logger.LogInformation("Create book success, Id: {Id}", result.Value);
        return result.Value;
    }

    private async Task AddChapter(long bookId, int order, string title, string content, CancellationToken ct = default)
    {
        using var logScope = logger.BeginScope("Add Chapter: {BookId}, {Order}, {Title}", bookId, order, title);
        using var scope    = scopeFactory.CreateScope();
        var       mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var command = new CreateChapterCommand(bookId, title, content, order);
        var result  = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            logger.LogInformation("Add chapter success");
        }
        else
        {
            logger.LogError("Failed to add chapter");
        }
    }
}

file class ResponseModel<T>
{
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string Message { get; set; } = null!;

    public T Data { get; set; } = default!;
}

file class PaginatedList<T>
{
    public int Count { get; set; }

    public int Page { get; set; }

    public int Size { get; set; }

    [JsonPropertyName("list")]
    public List<T> Data { get; set; } = [];
}

internal class BookInfo
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    [JsonPropertyName("des")]
    public string Description { get; set; } = null!;

    [JsonPropertyName("update_id")]
    public int LatestChapterOrder { get; set; }

    [JsonPropertyName("text")]
    public List<ChapterInfo> Chapters { get; set; } = [];
}

internal class ChapterInfo
{
    [JsonPropertyName("chapter_id")]
    public long Order { get; set; }

    [JsonPropertyName("tit")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("text")]
    public string? Content { get; set; }
}
