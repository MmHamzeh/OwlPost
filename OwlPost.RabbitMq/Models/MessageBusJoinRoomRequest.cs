namespace OwlPost.RabbitMq.Models;

public record MessageBusJoinRoomRequest : IMessageBusJoinRoomRequest
{
    public Guid RoomId { get; set; }

    public DateTime CreatedOn { get; init; }
    public Guid CreatedBy { get; init; }
    public string GroupingKey => "";
}