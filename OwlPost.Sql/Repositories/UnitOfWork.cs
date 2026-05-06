    using Microsoft.Extensions.DependencyInjection;

    namespace OwlPost.Sql.Repositories;

internal class UnitOfWork(OwlPostDbContext dbContext, IServiceProvider provider) : IUnitOfWork
{

    public IMessageRepository MessageRepository =>
        field ??= provider.GetRequiredService<IMessageRepository>();
    public IMessageHistoryRepository MessageHistoryRepository =>
        field ??= provider.GetRequiredService<IMessageHistoryRepository>();
    public IChatRoomRepository ChatRoomRepository =>
        field ??= provider.GetRequiredService<IChatRoomRepository>();




    #region Methods

    public async Task<int> SaveChanges(CancellationToken ct = default)
    {
        return await dbContext.SaveChangesAsync(ct);
    }

    public void RejectChanges()
    {
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    // revert property values to the original (database) values
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Added:
                    // new entities => simply forget them
                    entry.State = EntityState.Detached;
                    break;

                case EntityState.Deleted:
                    // restore deleted entities
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    // Unchanged or Detached => do nothing
                    break;
            }
        }
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await dbContext.DisposeAsync();
    }

    #endregion

}
