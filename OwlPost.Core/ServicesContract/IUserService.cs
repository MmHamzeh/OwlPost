namespace OwlPost.Core.ServicesContract;

public interface IUserService
{
    public Guid UserPublicId { get; }

    public long UserId { get; }
}