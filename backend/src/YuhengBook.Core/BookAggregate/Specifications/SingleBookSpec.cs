using Ardalis.Specification;

namespace YuhengBook.Core.BookAggregate;

public sealed class SingleBookSpec : Specification<Book>, ISingleResultSpecification<Book>
{
    public SingleBookSpec(long id, bool includeChapters = false)
    {
        Query.Where(b => b.Id == id);

        if (includeChapters)
        {
            Query.Include(b => b.Chapters.OrderBy(c => c.Order));
        }
    }
}
