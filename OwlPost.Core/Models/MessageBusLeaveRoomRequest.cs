namespace OwlPost.Core.Models;

public record MessageBusLeaveRoomRequest : IMessageBusRequest
{
    public MessageBusLeaveRoomRequest()
    {
        
    }

    public required DateTime CreatedOn { get; init; }
    public required Guid CreatedBy { get; init; }
    public required string GroupingKey { get; init; }
    public required Guid RoomId { get; init; }

    public void Deconstruct(out DateTime createdOn, out Guid createdBy, out string groupingKey, out Guid roomId)
    {
        createdOn = CreatedOn;
        createdBy = CreatedBy;
        groupingKey = GroupingKey;
        roomId = RoomId;
    }
}