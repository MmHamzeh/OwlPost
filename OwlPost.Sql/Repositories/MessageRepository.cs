namespace OwlPost.Sql.Repositories;

public class MessageRepository(OwlPostDbContext context) : IMessageRepository
{

    public async Task Add(ChatMessage message, bool saveChanges, CancellationToken ct)
    {
        await context.ChatMessages.AddAsync(message, ct);

        if (saveChanges)
            await context.SaveChangesAsync(ct);
    }
}