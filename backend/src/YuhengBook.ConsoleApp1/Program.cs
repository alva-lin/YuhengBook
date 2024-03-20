// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using YuhengBook.Core.BookAggregate;

Environment.CurrentDirectory = AppContext.BaseDirectory;

var random = new Random();

var logger = new LoggerConfiguration()
   .Enrich.FromLogContext()
   .WriteTo.Console()
   .WriteTo.File("logs/.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        fileSizeLimitBytes: 1024 * 1024
    )
   .CreateLogger();
var loggerFactory = new SerilogLoggerFactory(logger);

var crawlerService = new CrawlerService(loggerFactory.CreateLogger<CrawlerService>(), new HttpClient());
var yuhengBookApi  = new YuhengBookApi(loggerFactory.CreateLogger<YuhengBookApi>(), new HttpClient());


var source = new CancellationTokenSource();
var token  = source.Token;

await crawlerService.GetBookList(token);
var books = crawlerService.Books;
logger.Information("Start to crawl books. Count: {Count}", books.Count);
foreach (var book in books)
{
    var startOrder = 1;
    var (bookId, latestOrder) = await yuhengBookApi.GetBookCrawlerRecord(book.Title, token) ?? (0, 0);
    if (bookId > 0)
    {
        startOrder = (int)latestOrder + 1;
    }
    else
    {
        var newBookId = await yuhengBookApi.CreateBook(book.Title, book.Description, token);
        if (newBookId is null)
        {
            logger.Error("Failed to create book. Title: {Title}", book.Title);
            continue;
        }

        bookId = newBookId.Value;
    }

    if (startOrder > book.LatestChapterOrder)
    {
        continue;
    }

    for (var order = startOrder; order <= book.LatestChapterOrder; order++)
    {
        var chapter = await crawlerService.GetChapter(book.Id, order, token);
        if (chapter is not null)
        {
            await yuhengBookApi.AddChapter(bookId, order, chapter.Title, chapter.Content, token);
        }

        await Task.Delay(TimeSpan.FromSeconds(random.Next(1, 3)), token);
    }

    logger.Information("Crawl book finished. Title: {Title}", book.Title);
    await Task.Delay(TimeSpan.FromSeconds(random.Next(1, 3)), token);
}

logger.Information("Crawl books finished");

file class YuhengBookApi
{
    private const string BaseUrl = "http://localhost:57678";

    private readonly HttpClient             _client;
    private readonly ILogger<YuhengBookApi> _logger;

    public YuhengBookApi(ILogger<YuhengBookApi> logger, HttpClient client)
    {
        _logger = logger;
        _client = client;

        _client.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<(long BookId, long LatestOrder)?> GetBookCrawlerRecord(string title,
        CancellationToken ct = default)
    {
        var baseUrl = "/book";
        var param = new Dictionary<string, string?>
        {
            { "page", 1.ToString() },
            { "pageSize", 3.ToString() },
            { "keyword", title },
        };

        var url = QueryHelpers.AddQueryString(baseUrl, param);

        try
        {
            var resp = await _client.GetAsync(url, ct);
            if (resp.IsSuccessStatusCode)
            {
                var res = await resp.Content.ReadFromJsonAsync<YuhengBook.UseCases.PaginatedList<BookInfoDto>>(ct);
                if (res is not null)
                {
                    var book = res.Data.FirstOrDefault(b => b.Name == title);
                    if (book is not null)
                    {
                        _logger.LogInformation("Get Book Success. Title: {Title}", title);
                        return (book.Id, book.LastChapter?.Order ?? 0);
                    }

                    _logger.LogInformation("Book not found: {Title}", title);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get book record. Title: {Title}", title);
        }

        return null;
    }

    public async Task<long?> CreateBook(string title, string description, CancellationToken ct = default)
    {
        var url = "/book";
        var data = new
        {
            Name = title,
            Description = description,
        };

        try
        {
            _logger.LogInformation("Create Book... Title: {Title}", title);
            var resp = await _client.PostAsJsonAsync(url, data, cancellationToken: ct);
            if (resp.IsSuccessStatusCode)
            {
                var id = await resp.Content.ReadFromJsonAsync<long>(cancellationToken: ct);
                if (id <= 0)
                {
                    throw new("Failed to create book, id is 0");
                }

                _logger.LogInformation("Create Book Success. Title: {Title}, Id: {Id}", title, id);
                return id;

            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create book. Title: {Title}", title);
        }

        return null;
    }

    public async Task AddChapter(long bookId, int order, string title, string content, CancellationToken ct = default)
    {
        var url = $"/book/{bookId}/chapter";
        var data = new
        {
            Order = order,
            Title = title,
            Content = content,
        };

        try
        {
            _logger.LogInformation("Add Chapter... BookId: {BookId}, Order: {Order}", bookId, order);
            var resp = await _client.PostAsJsonAsync(url, data, cancellationToken: ct);
            if (resp.IsSuccessStatusCode)
            {
                _logger.LogInformation("Add Chapter Success. BookId: {BookId}, Order: {Order}", bookId, order);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to add chapter. BookId: {BookId}, Order: {Order}", bookId, order);
        }
    }
}


file class CrawlerService
{
    private const    string                  BaseUrl = "https://www.bqgnovels.com/";
    private readonly HttpClient              _client;
    private readonly ILogger<CrawlerService> _logger;

    public readonly List<BookInfo> Books = [];

    public CrawlerService(ILogger<CrawlerService> logger, HttpClient client)
    {
        _logger = logger;
        _client = client;

        _client.BaseAddress = new Uri(BaseUrl);
    }

    public async Task GetBookList(CancellationToken ct = default)
    {
        var path = "api/query/get_list";
        var param = new Dictionary<string, string?>
        {
            { "page", 1.ToString() },
            { "size", 200.ToString() },
        };

        var url = QueryHelpers.AddQueryString(path, param);

        try
        {
            _logger.LogInformation("Get Book List...");
            var resp = await _client.GetAsync(url, ct);
            if (resp.IsSuccessStatusCode)
            {
                var res = await resp.Content.ReadFromJsonAsync<ResponseModel<PaginatedList<BookInfo>>>(ct);
                if (res is not null)
                {
                    _logger.LogInformation("Get Book List Success. Count: {Count}", res.Data.Count);
                    Books.AddRange(res.Data.Data);
                }
                else
                {
                    _logger.LogWarning("Failed to get book list, response is null, url: {Url}", url);
                }
            }
            else
            {
                _logger.LogWarning("Failed to get book list, response is not success status code [{Code}], url: {Url}",
                    resp.StatusCode, resp.RequestMessage?.RequestUri?.AbsoluteUri);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get book list");
        }
    }

    public async Task<ChapterInfo?> GetChapter(long bookId, int order, CancellationToken ct = default)
    {
        var path = "api/query/get_book_text";
        var param = new Dictionary<string, string?>
        {
            { "bookId", bookId.ToString() },
            { "id", order.ToString() },
        };

        var url = QueryHelpers.AddQueryString(path, param);

        try
        {
            _logger.LogInformation("Get Chapter... BookId: {BookId}, Order: {Order}", bookId, order);
            var resp = await _client.GetAsync(url, ct);
            if (resp.IsSuccessStatusCode)
            {
                var res = await resp.Content.ReadFromJsonAsync<ResponseModel<BookInfo>>(ct);
                if (res is not null)
                {
                    var chapter = res.Data.Chapters.FirstOrDefault(c => c.Order == order);
                    if (chapter is not null)
                    {
                        _logger.LogInformation("Get Chapter Success. Title: {Title}", chapter.Title);
                        return chapter;
                    }
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get chapter");
        }

        return null;
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

file class BookInfo
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

file class ChapterInfo
{
    [JsonPropertyName("chapter_id")]
    public long Order { get; set; }

    [JsonPropertyName("tit")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("text")]
    public string Content { get; set; } = null!;
}

file class CreateBookResponse
{
    public long Id { get; set; }
}
