using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace YuhengBook.Infrastructure.Data;

public static class SeedData
{

    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var dbContext = new AppDbContext(
                   serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>(), null))
        {
            if (dbContext.Books.Any())
            {
                return; // DB has been seeded
            }

            PopulateTestData(dbContext);
        }
    }

    public static void PopulateTestData(AppDbContext dbContext)
    {
        foreach (var contributor in dbContext.Books)
        {
            dbContext.Remove(contributor);
        }

        dbContext.SaveChanges();

        dbContext.SaveChanges();
    }
}
