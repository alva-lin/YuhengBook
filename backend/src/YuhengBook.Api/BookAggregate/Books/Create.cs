using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Api.BookAggregate;

public sealed record CreateBookRequest
{
    public const string Route = "/Book";

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}

public sealed class CreateBookValidator : Validator<CreateBookRequest>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty()
           .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        RuleFor(x => x.Description)
           .MaximumLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);
    }
}

public class CreateBook(IMediator mediator)
    : Endpoint<CreateBookRequest, long>
{
    public override void Configure()
    {
        Post(CreateBookRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Create a book"; });
    }

    public override async Task HandleAsync(CreateBookRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateBookCommand(req.Name, req.Description), ct);

        this.CheckResult(result);
        await SendCreatedAtAsync<GetBook>(
            new { Id = result.Value },
            result.Value,
            cancellation: ct);
    }
}
