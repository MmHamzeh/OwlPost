namespace OwlPost.Core.ServicesContract;

public interface IMessageBus
{
    Task<IMessageBusResponse> SendMessage(IMessageBusSendMessageRequest request, CancellationToken ct);
    Task<IMessageBusResponse> DeleteMessage(IMessageBusDeleteMessageRequest request, CancellationToken ct);
    Task<IMessageBusResponse> EditMessage(IMessageBusEditMessageRequest request, CancellationToken ct);

    Task<IMessageBusResponse> JoinRoom(IMessageBusJoinRoomRequest request, CancellationToken ct);
    Task<IMessageBusResponse> LeaveRoom(IMessageBusLeaveRoomRequest request, CancellationToken ct);



}
