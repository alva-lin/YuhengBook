using YuhengBook.Core.BookAggregate;
using YuhengBook.UseCases;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Api.BookAggregate;

public sealed record ListBookRequest
{
    public const string Route = "/Book";

    public string? Keyword { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}

public sealed class ListBookValidator : Validator<ListBookRequest>
{
    public ListBookValidator()
    {
        RuleFor(x => x.Keyword)
           .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        RuleFor(x => x.Page)
           .GreaterThan(0)
            ;

        RuleFor(x => x.PageSize)
           .GreaterThan(0)
           .LessThan(DataSchemaConstants.PAGE_MAX_SIZE)
            ;
    }
}

public class ListBook(IMediator mediator)
    : Endpoint<ListBookRequest, Result<PaginatedList<BookInfoDto>>>
{
    public override void Configure()
    {
        Get(ListBookRequest.Route);
        AllowAnonymous();
        Summary(s => { s.Summary = "Get book list"; });
    }

    public override async Task HandleAsync(ListBookRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new ListBookQuery(req.PageSize, req.Page, req.Keyword?.Trim()), ct);

        this.CheckResult(result);
        await SendAsync(result, cancellation: ct);
    }
}
