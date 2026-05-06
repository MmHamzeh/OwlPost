namespace OwlPost.Core.Interfaces;

public interface IMessageBusRequest
{
    public Guid PublicId { get; set; }
    public DateTime Created { get; set; }


    public string MessageContent { get; set; }
    public bool IsPersistent { get; set; }

}
