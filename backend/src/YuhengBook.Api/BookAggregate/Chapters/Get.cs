using YuhengBook.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Api.BookAggregate.Chapters;

public sealed class GetChapterRequest
{
    public const string Route = "/Book/{bookId}/Chapter/{order}";

    public long BookId { get; set; }

    public int Order { get; set; }

    public static string BuildRoute(long bookId, int order) => Route
        .Replace("{bookId}", bookId.ToString())
        .Replace("{order}", order.ToString());
}

public class GetChapter(IMediator mediator)
    : Endpoint<GetChapterRequest, Result<ChapterDetailDto>>
{
    public override void Configure()
    {
        Get(GetChapterRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get chapter detail";
        });
    }

    public override async Task HandleAsync(GetChapterRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetChapterQuery(req.BookId, req.Order), ct);

        this.CheckResult(result);
        await SendAsync(result, cancellation: ct);
    }
}
