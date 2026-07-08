namespace OwlPost.RabbitMq.Models;

public record MessageBusDeleteMessageRequest : IMessageBusDeleteMessageRequest
{
    public Guid PublicId { get; set; }
    public Guid ConcurrencyToken { get; set; }

    public DateTime CreatedOn { get; init; }
    public Guid CreatedBy { get; init; }
    public string GroupingKey => "";
}