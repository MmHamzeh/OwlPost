namespace OwlPost.Core.Models;

public record MessageBusDeleteMessageRequest : IMessageBusRequest
{
    public MessageBusDeleteMessageRequest()
    {
        
    }

    public required DateTime CreatedOn { get; init; }
    public required long CreatedBy { get; init; }
    public required string GroupingKey { get; init; }
    public required Guid RoomId { get; init; }
    public required Guid ConcurrencyToken { get; init; }
    public required Guid MessageId { get; init; }

    public void Deconstruct(out DateTime createdOn, out long createdBy, out string groupingKey, out Guid roomId, out Guid concurrencyToken, out Guid messageId)
    {
        createdOn = CreatedOn;
        createdBy = CreatedBy;
        groupingKey = GroupingKey;
        roomId = RoomId;
        concurrencyToken = ConcurrencyToken;
        messageId = MessageId;
    }
}