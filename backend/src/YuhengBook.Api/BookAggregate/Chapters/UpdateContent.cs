using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Api.BookAggregate.Chapters;

public sealed class UpdateChapterDetailRequest
{
    public const string Route = "/Chapter/{id}/Detail";

    public long Id { get; set; }

    public string Content { get; set; } = null!;

    public static string BuildRoute(long id) => Route.Replace("{id}", id.ToString());
}

public sealed class UpdateChapterDetailValidator : Validator<UpdateChapterDetailRequest>
{
    public UpdateChapterDetailValidator()
    {
        RuleFor(x => x.Content)
           .NotEmpty();
    }
}

public class UpdateChapterDetail(IMediator mediator)
    : Endpoint<UpdateChapterDetailRequest>
{
    public override void Configure()
    {
        Put(UpdateChapterDetailRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Update chapter detail"; });
    }

    public override async Task HandleAsync(UpdateChapterDetailRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateChapterDetailCommand(req.Id, req.Content), ct);

        this.CheckResult(result);
        await SendNoContentAsync(cancellation: ct);
    }
}
