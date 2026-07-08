namespace OwlPost.RabbitMq.Models;

public record MessageBusEditMessageRequest : IMessageBusEditMessageRequest
{
    public Guid PublicId { get; set; }
    public required string Content { get; set; }
    public Guid ConcurrencyToken { get; set; }

    public DateTime CreatedOn { get; init; }
    public Guid CreatedBy { get; init; }
    public string GroupingKey => "";
}