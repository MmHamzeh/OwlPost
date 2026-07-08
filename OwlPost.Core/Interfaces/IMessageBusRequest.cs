namespace OwlPost.Core.Interfaces;

public interface IMessageBusRequest
{
    public DateTime CreatedOn { get; init; }
    public Guid CreatedBy { get; init; }
    public string GroupingKey { get; }

}

public interface IMessageBusSendMessageRequest : IMessageBusRequest
{
    public string Content { get; set; }
}

public interface IMessageBusEditMessageRequest : IMessageBusRequest
{
    public Guid PublicId { get; set; }
    public string Content { get; set; }
    public Guid ConcurrencyToken { get; set; }
}

public interface IMessageBusDeleteMessageRequest : IMessageBusRequest
{
    public Guid PublicId { get; set; }
    public Guid ConcurrencyToken { get; set; }
}

public interface IMessageBusJoinRoomRequest : IMessageBusRequest
{
    public Guid RoomId { get; set; }
}

public interface IMessageBusLeaveRoomRequest : IMessageBusRequest
{
    public Guid RoomId { get; set; }
}