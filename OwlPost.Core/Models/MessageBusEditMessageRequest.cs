namespace OwlPost.Core.Models;

public record MessageBusEditMessageRequest : IMessageBusRequest
{
    public MessageBusEditMessageRequest()
    {
        
    }

    public required DateTime CreatedOn { get; init; }
    public required long CreatedBy { get; init; }
    public required string GroupingKey { get; init; }
    public required string Content { get; init; }
    public required Guid RoomId { get; init; }
    public required Guid ConcurrencyToken { get; init; }
    public required Guid MessageId { get; init; }

    public void Deconstruct(out DateTime createdOn, out long createdBy, out string groupingKey, out string content, out Guid roomId, out Guid concurrencyToken, out Guid messageId)
    {
        createdOn = CreatedOn;
        createdBy = CreatedBy;
        groupingKey = GroupingKey;
        content = Content;
        roomId = RoomId;
        concurrencyToken = ConcurrencyToken;
        messageId = MessageId;
    }
}