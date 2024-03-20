using Microsoft.EntityFrameworkCore;
using YuhengBook.Core.BookAggregate;

namespace YuhengBook.Infrastructure.Data;

public sealed partial class AppDbContext
{
    public DbSet<Book> Books => Set<Book>();
}
