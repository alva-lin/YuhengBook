using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Api.BookAggregate.Chapters;

public sealed record CreateChapterRequest
{
    public const string Route = "/Book/{bookId}/Chapter";

    public long BookId { get; set; }

    public int? Order { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public static string BuildRoute(long bookId) => Route.Replace("{bookId}", bookId.ToString());
}

public sealed class CreateChapterValidator : Validator<CreateChapterRequest>
{
    public CreateChapterValidator()
    {
        RuleFor(x => x.BookId)
           .GreaterThan(0);

        RuleFor(x => x.Title)
           .NotEmpty()
           .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        RuleFor(x => x.Content)
           .NotEmpty();
    }
}

public class CreateChapter(IMediator mediator)
    : Endpoint<CreateChapterRequest, Result<long>>
{
    public override void Configure()
    {
        Post(CreateChapterRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Add a new Chapter to a Book"; });
    }

    public override async Task HandleAsync(CreateChapterRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateChapterCommand(req.BookId, req.Title, req.Content, req.Order), ct);

        this.CheckResult(result);
        await SendAsync(result, cancellation: ct);
    }
}
