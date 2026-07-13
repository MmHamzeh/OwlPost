using OwlPost.Core.Interfaces;
using OwlPost.RabbitMq.Models;

namespace OwlPost.Services;

public class MessagingService(
    IMessageBus messageBus,
    ISanitizer sanitizer,
    TimeProvider timeProvider,
    IChatRoomRepository chatRoomRepository,
    IUserService userService)
{
    public async Task SendMessage(SendMessageDto dto, CancellationToken ct)
    {
        var now = timeProvider.GetUtcNow();

        var chatRoomId =
            await chatRoomRepository
                .DoesUserHaveAccessToRoom(dto.RoomId, userService.UserPublicId, ct);

        if (chatRoomId is null)
        {
            throw new Exception("you have no access to this chatroom");
        }

        var sanitizedContent = sanitizer.Sanitize(dto.Content);

        var groupingKey = dto.RoomId.ToString();

        var req = new MessageBusSendMessageRequest(now.DateTime, userService.UserPublicId, groupingKey,
            sanitizedContent, dto.RoomId);
        

        await messageBus.SendMessage(req, ct);

        //TODO: this is for consumer part
        //var msg = new ChatMessage()
        //{
        //    CreatedOn = now.DateTime,
        //    CreatedBy = userService.UserId,
        //    Content = sanitizedContent,
        //    PublicId = Guid.CreateVersion7(now),
        //    ChatRoomId = chatRoomId.Value,
        //    ParentMessageId = null,
        //};
        //await messageRepository.Add(msg, saveChanges: true, ct);

    }


}