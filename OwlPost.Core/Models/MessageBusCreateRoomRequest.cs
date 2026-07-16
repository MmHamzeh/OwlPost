namespace OwlPost.Core.Models;

public record MessageBusCreateRoomRequest : IMessageBusRequest
{
    public MessageBusCreateRoomRequest()
    {
        
    }

    public required DateTime CreatedOn { get; init; }
    public required long CreatedBy { get; init; }
    public required string GroupingKey { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    public void Deconstruct(out DateTime createdOn, out long createdBy, out string groupingKey, out string name, out string description)
    {
        createdOn = CreatedOn;
        createdBy = CreatedBy;
        groupingKey = GroupingKey;
        name = Name;
        description = Description;
    }
}