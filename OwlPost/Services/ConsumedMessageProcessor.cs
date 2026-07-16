namespace OwlPost.Services;


public sealed class ConsumedMessageProcessor(
    ChatMessageService chatMessageService,
    IChatHubService chatHubService,
    ILogger<ConsumedMessageProcessor> logger) : IConsumedMessageProcessor
{
    public async Task ProcessAsync(IMessageBusRequest request, CancellationToken ct = default)
    {
        switch (request)
        {
            case MessageBusSendMessageRequest sendMessage:
                await HandleSendMessageAsync(sendMessage, ct);
                break;

            case MessageBusEditMessageRequest editMessage:
                await HandleEditMessageAsync(editMessage, ct);
                break;

            case MessageBusDeleteMessageRequest deleteMessage:
                await HandleDeleteMessageAsync(deleteMessage, ct);
                break;

            case MessageBusCreateRoomRequest createRoom:
                await HandleCreateRoomAsync(createRoom, ct);
                break;

            case MessageBusEditRoomRequest editRoom:
                await HandleEditRoomAsync(editRoom, ct);
                break;

            case MessageBusDeleteRoomRequest deleteRoom:
                await HandleDeleteRoomAsync(deleteRoom, ct);
                break;

            case MessageBusJoinRoomRequest joinRoom:
                await HandleJoinRoomAsync(joinRoom, ct);
                break;

            case MessageBusLeaveRoomRequest leaveRoom:
                await HandleLeaveRoomAsync(leaveRoom, ct);
                break;

            default:
                throw new NotSupportedException(
                    $"Unsupported message type: {request.GetType().FullName}");
        }
    }



    private async Task HandleSendMessageAsync(MessageBusSendMessageRequest request, CancellationToken ct)
    {
        logger.LogTrace("Processing SendMessage for room {RoomId}", request.RoomId);

        var entity = new ChatMessage
        {
            PublicId = Guid.CreateVersion7(),
            Content = request.Content,
            CreatedOn = request.CreatedOn,
            CreatedBy = 0, // if your entity type is long, this must be mapped
            Version = Guid.CreateVersion7()
        };

        await chatMessageService.AddNewMessage(entity, ct);
        await chatHubService.NotifyMessageAdded(entity, ct);

        logger.LogTrace("SendMessage processed for room {RoomId}", request.RoomId);
    }

    private async Task HandleEditMessageAsync(MessageBusEditMessageRequest request,CancellationToken ct)
    {
        logger.LogInformation("Processing EditMessage for message {MessageId}", request.MessageId);

        // TODO:
        // - load existing message
        // - validate concurrency token
        // - update content
        // - save changes
        // - notify SignalR

        await Task.CompletedTask;
    }

    private async Task HandleDeleteMessageAsync(MessageBusDeleteMessageRequest request, CancellationToken ct)
    {
        logger.LogInformation("Processing DeleteMessage for message {MessageId}", request.MessageId);

        // TODO:
        // - load existing message
        // - validate concurrency token
        // - delete message
        // - save changes
        // - notify SignalR

        await Task.CompletedTask;
    }

    private async Task HandleCreateRoomAsync(MessageBusCreateRoomRequest request, CancellationToken ct)
    {
        logger.LogInformation("Processing CreateRoom: {Name}", request.Name);

        // TODO:
        // - create room entity
        // - save changes
        // - notify SignalR

        await Task.CompletedTask;
    }

    private async Task HandleEditRoomAsync(MessageBusEditRoomRequest request, CancellationToken ct)
    {
        logger.LogInformation("Processing EditRoom for room {RoomId}", request.RoomId);

        // TODO:
        // - load room
        // - update name/description
        // - save changes
        // - notify SignalR

        await Task.CompletedTask;
    }

    private async Task HandleDeleteRoomAsync(MessageBusDeleteRoomRequest request, CancellationToken ct)
    {
        logger.LogInformation("Processing DeleteRoom for room {RoomId}", request.RoomId);

        // TODO:
        // - delete room
        // - save changes
        // - notify SignalR

        await Task.CompletedTask;
    }

    private async Task HandleJoinRoomAsync(MessageBusJoinRoomRequest request, CancellationToken ct)
    {
        logger.LogInformation("Processing JoinRoom for room {RoomId}", request.RoomId);

        // TODO:
        // - add membership
        // - save changes
        // - notify SignalR

        await Task.CompletedTask;
    }

    private async Task HandleLeaveRoomAsync(MessageBusLeaveRoomRequest request, CancellationToken ct)
    {
        logger.LogInformation("Processing LeaveRoom for room {RoomId}", request.RoomId);

        // TODO:
        // - remove membership
        // - save changes
        // - notify SignalR

        await Task.CompletedTask;
    }


}
