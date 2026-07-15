namespace OwlPost.Services;

public class MessagingService(
    IMessageBus messageBus,
    ISanitizer sanitizer,
    TimeProvider timeProvider,
    IChatRoomRepository chatRoomRepository,
    IMessageRepository messageRepository,
    IUserService userService)
{
    public async Task<ApiResponse> SendMessage(SendMessageDto dto, CancellationToken ct)
    {
        var chatRoomId = await chatRoomRepository
                .DoesUserHaveAccessToRoom(dto.RoomId, userService.UserPublicId, ct);

        if (chatRoomId is null)
            throw new Exception("you have no access to this chatroom");

        var sanitizedContent = sanitizer.Sanitize(dto.Content);

        var groupingKey = dto.RoomId.ToString();

        var req = new MessageBusSendMessageRequest
        {
            CreatedOn = timeProvider.GetUtcNow().DateTime, 
            CreatedBy = userService.UserPublicId,
            GroupingKey = groupingKey,
            Content = sanitizedContent,
            RoomId = dto.RoomId
        };
        
        await messageBus.SendMessage(req, ct);

        return new ApiResponse();
    }


    public async Task<ApiResponse> EditMessage(EditMessageDto dto, CancellationToken ct)
    {
        var chatRoomId = await chatRoomRepository
                .DoesUserHaveAccessToRoom(dto.RoomId, userService.UserPublicId, ct);
        
        if (chatRoomId is null)
            throw new Exception("you have no access to this chatroom");

        var concurrencyToken = await messageRepository
                .GetUserMessageConcurrencyToken(dto.RoomId, dto.MessageId, userService.UserPublicId, ct);

        if (concurrencyToken is null)
            throw new Exception("you have no access to this Message");

        var sanitizedContent = sanitizer.Sanitize(dto.Content);

        var groupingKey = dto.RoomId.ToString();

        var req = new MessageBusEditMessageRequest
        {
            CreatedOn = timeProvider.GetUtcNow().DateTime,
            CreatedBy = userService.UserPublicId,
            GroupingKey = groupingKey,
            Content = sanitizedContent,
            RoomId = dto.RoomId,
            ConcurrencyToken = concurrencyToken.Value,
            MessageId = dto.MessageId
        };

        await messageBus.EditMessage(req, ct);
        return new ApiResponse();
    }

    public async Task<ApiResponse> DeleteMessage(DeleteMessageDto dto, CancellationToken ct)
    {
        var chatRoomId =
            await chatRoomRepository
                .DoesUserHaveAccessToRoom(dto.RoomId, userService.UserPublicId, ct);

        if (chatRoomId is null)
            throw new Exception("you have no access to this chatroom");

        var concurrencyToken =
            await messageRepository
                .GetUserMessageConcurrencyToken(dto.RoomId, dto.MessageId, userService.UserPublicId, ct);

        if (concurrencyToken is null)
            throw new Exception("you have no access to this Message");

        var groupingKey = dto.RoomId.ToString();

        var req = new MessageBusDeleteMessageRequest
        {
            CreatedOn = timeProvider.GetUtcNow().DateTime,
            CreatedBy = userService.UserPublicId,
            GroupingKey = groupingKey,
            RoomId = dto.RoomId,
            ConcurrencyToken = concurrencyToken.Value,
            MessageId = dto.MessageId
        };

        await messageBus.DeleteMessage(req, ct);
        return new ApiResponse();
    }
}