namespace OwlPost.Core.ServicesContract;

public interface IMessageBus
{
    Task<IMessageBusResponse> SendMessage(IMessageBusSendMessageRequest request);
    Task<IMessageBusResponse> DeleteMessage(IMessageBusDeleteMessageRequest request);
    Task<IMessageBusResponse> EditMessage(IMessageBusEditMessageRequest request);

    Task<IMessageBusResponse> JoinRoom(IMessageBusJoinRoomRequest request);
    Task<IMessageBusResponse> LeaveRoom(IMessageBusLeaveRoomRequest request);



}
