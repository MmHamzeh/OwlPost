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

    public async Task<ChatMessage?> GetById(long id, bool enableTracking)
    {
        var query = context.ChatMessages.Where(e => e.Id == id);
        return await (enableTracking 
            ? query.FirstOrDefaultAsync()
            : query.AsNoTracking().FirstOrDefaultAsync());
    }

    public async Task<ChatMessage?> GetByPublicId(Guid id, bool enableTracking)
    {
        var query = context.ChatMessages.Where(e => e.PublicId == id);
        return await (enableTracking 
            ? query.FirstOrDefaultAsync()
            : query.AsNoTracking().FirstOrDefaultAsync());
    }
}