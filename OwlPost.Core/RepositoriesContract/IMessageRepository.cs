namespace OwlPost.Core.RepositoriesContract;

public interface IMessageRepository
{
    Task Add(ChatMessage message, CancellationToken ct);
    Task<Guid?> GetUserMessageConcurrencyToken(Guid roomPublicId, Guid messagePublicId, Guid userPublicId, CancellationToken ct);
}