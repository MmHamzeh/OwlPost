namespace OwlPost.Core.ServicesContract;

public interface IMessageRepository
{
    Task<int> Add(ChatMessage message);
}