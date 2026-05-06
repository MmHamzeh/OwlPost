namespace OwlPost.Core.RepositoriesContract;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    public IMessageRepository MessageRepository { get; }
    public IMessageHistoryRepository MessageHistoryRepository { get; }
    public IChatRoomRepository ChatRoomRepository { get; }




    
    #region Methods

    Task<int> SaveChanges(CancellationToken ct = default);
    void RejectChanges();

    #endregion

}