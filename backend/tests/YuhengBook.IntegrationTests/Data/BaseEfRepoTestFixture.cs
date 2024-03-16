using Ardalis.SharedKernel;
using YuhengBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using YuhengBook.Core.BookAggregate;

namespace YuhengBook.IntegrationTests.Data;

public abstract class BaseEfRepoTestFixture
{
    protected readonly AppDbContext DbContext;

    public readonly Faker Fake;

    protected BaseEfRepoTestFixture()
    {
        var options             = CreateNewContextOptions();
        var fakeEventDispatcher = Substitute.For<IDomainEventDispatcher>();

        DbContext = new(options, fakeEventDispatcher);
        Fake = new();
    }

    protected static DbContextOptions<AppDbContext> CreateNewContextOptions()
    {
        // Create a fresh service provider, and therefore a fresh
        // InMemory database instance.
        var serviceProvider = new ServiceCollection()
           .AddEntityFrameworkInMemoryDatabase()
           .BuildServiceProvider();

        // Create a new options instance telling the context to use an
        // InMemory database and the new service provider.
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseInMemoryDatabase("cleanarchitecture")
           .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }

    protected EfRepository<Book> GetRepository() => new(DbContext);
}
