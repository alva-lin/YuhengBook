using Ardalis.Specification;

namespace YuhengBook.Core.BookAggregate;

public sealed class SingleChapterSpec :
    Specification<Chapter>,
    ISingleResultSpecification<Chapter>
{
    public SingleChapterSpec(long id, bool includeDetail = false)
    {
        Query.Where(c => c.Id == id);

        if (includeDetail)
        {
            Query.Include(c => c.Detail);
        }
    }

    public SingleChapterSpec(long bookId, int order, bool includeDetail = false)
    {
        Query.Where(c => c.Book.Id == bookId)
           .Where(c => c.Order == order)
            ;

        if (includeDetail)
        {
            Query.Include(c => c.Detail);
        }
    }
}
