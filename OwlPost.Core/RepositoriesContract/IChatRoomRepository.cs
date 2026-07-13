namespace OwlPost.Core.RepositoriesContract;

public interface IChatRoomRepository
{
    Task<long?> DoesUserHaveAccessToRoom(Guid roomId, Guid userId, CancellationToken ct);
}