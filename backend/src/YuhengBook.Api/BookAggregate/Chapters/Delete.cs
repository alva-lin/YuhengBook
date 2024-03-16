using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Api.BookAggregate.Chapters;

public sealed record DeleteChapterRequest
{
    public const string Route = "/Chapter/{id}";

    public long Id { get; set; }

    public static string BuildRoute(long id) => Route.Replace("{id}", id.ToString());
}

public class DeleteChapter(IMediator mediator)
    : Endpoint<DeleteChapterRequest>
{
    public override void Configure()
    {
        Delete(DeleteChapterRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Delete chapter";
        });
    }

    public override async Task HandleAsync(DeleteChapterRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteChapterCommand(req.Id), ct);

        this.CheckResult(result);
        await SendNoContentAsync(cancellation: ct);
    }
}
