namespace OwlPost.Sql.Repositories;

public class MessageHistoryRepository(OwlPostDbContext context) : IMessageHistoryRepository
{
    public async Task Add(ChatMessageHistory chatMessageHistory, CancellationToken ct)
    {
        await context.ChatMessageHistories.AddAsync(chatMessageHistory, ct);
    }
}