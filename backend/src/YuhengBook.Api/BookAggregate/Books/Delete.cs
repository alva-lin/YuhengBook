using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Api.BookAggregate;

public sealed class DeleteBookRequest
{
    public const string Route = "/Book/{id}";

    public long Id { get; set; }

    public static string BuildRoute(long id) => Route.Replace("{id}", id.ToString());
}

public class DeleteBook(IMediator mediator)
    : Endpoint<DeleteBookRequest>
{
    public override void Configure()
    {
        Delete(DeleteBookRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Delete book"; });
    }

    public override async Task HandleAsync(DeleteBookRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteBookCommand(req.Id), ct);

        this.CheckResult(result);
        await SendNoContentAsync(ct);
    }
}
