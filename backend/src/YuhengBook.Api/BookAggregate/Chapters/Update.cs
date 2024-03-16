using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Api.BookAggregate.Chapters;

public sealed record UpdateChapterRequest
{
    public const string Route = "/Chapter/{id}";

    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public static string BuildRoute(long id) => Route.Replace("{id}", id.ToString());
}

public sealed class UpdateChapterValidator : Validator<UpdateChapterRequest>
{
    public UpdateChapterValidator()
    {
        RuleFor(x => x.Title)
           .NotEmpty()
           .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
    }
}

public class UpdateChapter(IMediator mediator)
    : Endpoint<UpdateChapterRequest>
{
    public override void Configure()
    {
        Put(UpdateChapterRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Update chapter";
        });
    }

    public override async Task HandleAsync(UpdateChapterRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateChapterCommand(req.Id, req.Title), ct);

        this.CheckResult(result);
        await SendNoContentAsync(cancellation: ct);
    }
}
