namespace OwlPost.Services;

public class ChatMessageService(
    IUnitOfWork unitOfWork,
    IMessageRepository messageRepository,
    ILogger<ChatMessageService> logger)
{
    public async Task AddNewMessage(ChatMessage message, CancellationToken ct)
    {

    }
}