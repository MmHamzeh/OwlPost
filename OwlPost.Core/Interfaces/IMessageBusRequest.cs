namespace OwlPost.Core.Interfaces;

public interface IMessageBusRequest
{
    public DateTime CreatedOn { get; init; }
    public Guid CreatedBy { get; init; }
    public string GroupingKey { get; init; }

}