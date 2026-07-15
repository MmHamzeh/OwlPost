namespace OwlPost.Core.RepositoriesContract;

public interface IMessageRepository
{
    Task Add(ChatMessage message, bool saveChanges, CancellationToken ct);
    Task<Guid?> GetUserMessageConcurrencyToken(Guid roomPublicId, Guid messagePublicId, Guid userPublicId, CancellationToken ct);
}