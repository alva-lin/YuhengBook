using System.Reflection;
using Ardalis.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace YuhengBook.Infrastructure.Data;

public sealed partial class AppDbContext : DbContext
{
    private readonly IDomainEventDispatcher? _dispatcher;

    public AppDbContext(DbContextOptions<AppDbContext> options,
        IDomainEventDispatcher? dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;

        // ChangeTracker.StateChanged += SetPartialUpdate;
        // ChangeTracker.Tracked += SetPartialUpdate;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // ignore events if no dispatcher provided
        if (_dispatcher == null)
        {
            return result;
        }

        // dispatch events only if save was successful
        var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
           .Select(e => e.Entity)
           .Where(e => e.DomainEvents.Any())
           .ToArray();

        await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

        return result;
    }

    public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();

    // TODO - 部分更新
    private void SetPartialUpdate(object? sender, EntityEntryEventArgs e)
    {
        if (e.Entry.State != EntityState.Modified)
        {
            return;
        }

        foreach (var propertyEntry in e.Entry.Properties.Where(pEntry => pEntry.IsModified))
        {
            if (Equals(propertyEntry.OriginalValue, propertyEntry.CurrentValue))
            {
                propertyEntry.IsModified = false;
            }
        }

        if (!e.Entry.Properties.Any(pEntry => pEntry.IsModified))
        {
            e.Entry.State = EntityState.Unchanged;
        }
    }
}
