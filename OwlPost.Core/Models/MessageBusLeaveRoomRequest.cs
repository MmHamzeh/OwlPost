namespace OwlPost.Core.Models;

public record MessageBusLeaveRoomRequest : IMessageBusRequest
{
    public MessageBusLeaveRoomRequest()
    {
        
    }

    public required DateTime CreatedOn { get; init; }
    public required long CreatedBy { get; init; }
    public required string GroupingKey { get; init; }
    public required Guid RoomId { get; init; }

    public void Deconstruct(out DateTime createdOn, out long createdBy, out string groupingKey, out Guid roomId)
    {
        createdOn = CreatedOn;
        createdBy = CreatedBy;
        groupingKey = GroupingKey;
        roomId = RoomId;
    }
}