namespace OwlPost.Core.Interfaces;

public interface IMessageBusRequest
{
    public DateTime CreatedOn { get; init; }
    public Guid CreatedBy { get; init; }
    public string GroupingKey { get; }

}

public record MessageBusSendMessageRequest(DateTime CreatedOn, Guid CreatedBy, string GroupingKey, string Content, Guid RoomId) 
    : IMessageBusRequest
{
}

public record MessageBusEditMessageRequest(DateTime CreatedOn, Guid CreatedBy, string GroupingKey, string Content, Guid RoomId, Guid ConcurrencyToken, Guid MessageId) 
    : IMessageBusRequest
{
}

public record MessageBusDeleteMessageRequest(DateTime CreatedOn, Guid CreatedBy, string GroupingKey, Guid RoomId, Guid ConcurrencyToken, Guid MessageId)
    : IMessageBusRequest
{
}

public record MessageBusJoinRoomRequest(DateTime CreatedOn, Guid CreatedBy, string GroupingKey, Guid RoomId) 
    : IMessageBusRequest
{
}

public record MessageBusLeaveRoomRequest(DateTime CreatedOn, Guid CreatedBy, string GroupingKey, Guid RoomId) 
    : IMessageBusRequest
{
}

public record MessageBusCreateRoomRequest(DateTime CreatedOn, Guid CreatedBy, string GroupingKey, string Name, string Description)
    : IMessageBusRequest
{
}

public record MessageBusEditRoomRequest(DateTime CreatedOn, Guid CreatedBy, string GroupingKey, Guid RoomId, string Name, string Description)
    : IMessageBusRequest
{
}

public record MessageBusDeleteRoomRequest(DateTime CreatedOn, Guid CreatedBy, string GroupingKey, Guid RoomId)
    : IMessageBusRequest
{
}