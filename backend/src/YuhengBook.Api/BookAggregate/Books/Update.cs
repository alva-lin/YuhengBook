using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Api.BookAggregate;

public sealed record UpdateBookRequest
{
    public const string Route = "Book/{id}";

    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public static string BuildRoute(long id) => Route.Replace("{id}", id.ToString());
}

public sealed class UpdateBookValidator : Validator<UpdateBookRequest>
{
    public UpdateBookValidator()
    {
        RuleFor(e => e.Name)
           .NotEmpty()
           .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
            ;

        RuleFor(e => e.Description)
           .MaximumLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH)
            ;
    }
}

public class UpdateBook(IMediator mediator)
    : Endpoint<UpdateBookRequest>
{
    public override void Configure()
    {
        Put(UpdateBookRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Update book"; });
    }

    public override async Task HandleAsync(UpdateBookRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateBookCommand(req.Id, req.Name, req.Description), ct);

        this.CheckResult(result);
        await SendNoContentAsync(ct);
    }
}
