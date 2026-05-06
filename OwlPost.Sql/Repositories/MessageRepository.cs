namespace OwlPost.Sql.Repositories;

public class MessageRepository(OwlPostDbContext context) : IMessageRepository
{

    public async Task Add(ChatMessage message, CancellationToken ct)
    {
        await context.ChatMessages.AddAsync(message, ct);
    }
}