namespace OwlPost.Core.ServicesContract;

public interface IMessageBusRequest
{
    public Guid PublicId { get; set; }
    public DateTime Created { get; set; }


    public string MessageContent { get; set; }
    public string Exchange { get; set; }
    public string RoutingKey { get; set; }
    public bool IsPersistent { get; set; }

}
