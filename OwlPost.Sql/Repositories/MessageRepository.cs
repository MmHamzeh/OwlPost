namespace OwlPost.Sql.Repositories;

public class MessageRepository(OwlPostDbContext context) : IMessageRepository
{

    public async Task Add(ChatMessage message, CancellationToken ct)
    {
        await context.ChatMessages.AddAsync(message, ct);
    }

    public async Task<Guid?> GetUserMessageConcurrencyToken(Guid roomPublicId, Guid messagePublicId, Guid userPublicId, CancellationToken ct)
    {
        return await context.ChatMessages.Where(e =>
                e.ChatRoom.PublicId == roomPublicId &&
                e.PublicId == messagePublicId &&
                e.User.PublicId == userPublicId)
            .Select(e => e.Version)
            .FirstOrDefaultAsync(ct);

    }
}