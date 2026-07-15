namespace OwlPost.Core.Models;

public record MessageBusEditRoomRequest : IMessageBusRequest
{
    public MessageBusEditRoomRequest()
    {
        
    }

    public required DateTime CreatedOn { get; init; }
    public required Guid CreatedBy { get; init; }
    public required string GroupingKey { get; init; }
    public required Guid RoomId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    public void Deconstruct(out DateTime createdOn, out Guid createdBy, out string groupingKey, out Guid roomId, out string name, out string description)
    {
        createdOn = CreatedOn;
        createdBy = CreatedBy;
        groupingKey = GroupingKey;
        roomId = RoomId;
        name = Name;
        description = Description;
    }
}