namespace OwlPost.Sql.Repositories;

public class MessageRepository(OwlPostDbContext context) : IMessageRepository
{

    public async Task Add(ChatMessage message, bool saveChanges, CancellationToken ct)
    {
        await context.ChatMessages.AddAsync(message, ct);

        if (saveChanges)
            await context.SaveChangesAsync(ct);
    }

    public async Task<Guid?> GetMessageConcurrencyToken(Guid RoomPublicId, Guid messagePublicId, Guid userPublicId, CancellationToken ct)
    {
        return await context.ChatMessages.Where(e =>
                e.ChatRoom.PublicId == RoomPublicId &&
                e.PublicId == messagePublicId &&
                e.User.PublicId == userPublicId)
            .Select(e => e.Version)
            .FirstOrDefaultAsync(ct);

    }
}