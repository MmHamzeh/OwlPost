namespace OwlPost.RabbitMq.Models;

public record MessageBusSendMessageRequest : IMessageBusSendMessageRequest
{
    public DateTime CreatedOn { get; init; }
    public Guid CreatedBy { get; init; }
    public string GroupingKey => "";

    public required string Content { get; set; }
}