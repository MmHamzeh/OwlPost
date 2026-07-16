namespace OwlPost.Core.Models;

public record MessageBusEditRoomRequest : IMessageBusRequest
{
    public MessageBusEditRoomRequest()
    {
        
    }

    public required DateTime CreatedOn { get; init; }
    public required long CreatedBy { get; init; }
    public required string GroupingKey { get; init; }
    public required Guid RoomId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    public void Deconstruct(out DateTime createdOn, out long createdBy, out string groupingKey, out Guid roomId, out string name, out string description)
    {
        createdOn = CreatedOn;
        createdBy = CreatedBy;
        groupingKey = GroupingKey;
        roomId = RoomId;
        name = Name;
        description = Description;
    }
}