namespace OwlPost.Services;

public class ChatMessageService(
    IUnitOfWork unitOfWork,
    IMessageRepository messageRepository,
    IMessageHistoryRepository messageHistoryRepository,
    IMessagingPermissionService messagingPermissionService,
    ILogger<ChatMessageService> logger,
    TimeProvider timeProvider,
    IUserService userService)
{
    public async Task AddNewMessage(ChatMessage message, CancellationToken ct)
    {
        await messageRepository.Add(message, ct);
        await unitOfWork.SaveChanges(ct);
    }

    public async Task EditMessage(Guid messageGuid, string content, CancellationToken ct)
    {
        var dbMessage = await messageRepository.GetByPublicId(messageGuid, enableTracking: true);

        if (dbMessage == null)
            throw new InvalidOperationException("Message not found");

        var canUserEditMessage = await messagingPermissionService.CanEditMessageAsync(dbMessage.Id, userService.UserId, dbMessage.ChatRoomId, ct);

        if (!canUserEditMessage)
            throw new InvalidOperationException("User is not the author of the message");

        var messageHistory = new ChatMessageHistory(dbMessage, userService.UserId, timeProvider);

        dbMessage.Content = content;
        dbMessage.ModifiedOn = timeProvider.GetUtcNow().DateTime;
        dbMessage.ModifiedBy = userService.UserId;

        await messageHistoryRepository.Add(messageHistory, ct);
        await unitOfWork.SaveChanges(ct);
    }

    public async Task DeleteMessage(Guid messageGuid, CancellationToken ct)
    {
        var dbMessage = await messageRepository.GetByPublicId(messageGuid, enableTracking: true);

        if (dbMessage == null)
            throw new InvalidOperationException("Message not found");

        var canUserDeleteMessage = await messagingPermissionService.CanDeleteMessageAsync(dbMessage.Id, userService.UserId, dbMessage.ChatRoomId, ct);

        if (!canUserDeleteMessage)
            throw new InvalidOperationException("User is not the author of the message");

        var messageHistory = new ChatMessageHistory(dbMessage, userService.UserId, timeProvider);
        dbMessage.IsDeleted = true;
        dbMessage.ModifiedOn = timeProvider.GetUtcNow().DateTime;
        dbMessage.ModifiedBy = userService.UserId;

        await messageHistoryRepository.Add(messageHistory, ct);
        await unitOfWork.SaveChanges(ct);
    }
}