namespace OwlPost.Core.ServicesContract;

public interface IMessageBus
{
    Task<IMessageBusResponse> SendMessage(MessageBusSendMessageRequest request, CancellationToken ct);


    Task<IMessageBusResponse> DeleteMessage(MessageBusDeleteMessageRequest request, CancellationToken ct);
    Task<IMessageBusResponse> EditMessage(MessageBusEditMessageRequest request, CancellationToken ct);

    Task<IMessageBusResponse> JoinRoom(MessageBusJoinRoomRequest request, CancellationToken ct);
    Task<IMessageBusResponse> LeaveRoom(MessageBusLeaveRoomRequest request, CancellationToken ct);

}
