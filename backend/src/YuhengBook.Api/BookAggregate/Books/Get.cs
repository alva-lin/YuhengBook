using YuhengBook.Core.BookAggregate;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Api.BookAggregate;

public sealed record GetBookRequest
{
    public const string Route = "/Book/{id}";

    public long Id { get; set; }

    public static string GetRoute(long id) => Route.Replace("{id}", id.ToString());
}

public class GetBook(IMediator mediator)
    : Endpoint<GetBookRequest, BookDetailDto>
{
    public override void Configure()
    {
        Get(GetBookRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Get a Book Info"; });
    }

    public override async Task HandleAsync(GetBookRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetBookQuery(req.Id), ct);

        this.CheckResult(result);
        await SendAsync(result.Value, cancellation: ct);
    }
}
