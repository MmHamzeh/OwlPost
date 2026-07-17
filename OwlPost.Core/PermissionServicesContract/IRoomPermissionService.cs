namespace OwlPost.Core.PermissionServicesContract;

public interface IRoomPermissionService
{
    Task<bool> CanCreateRoomAsync(long userId, CancellationToken ct);
    Task<bool> CanEditRoomAsync(long roomId, long userId, CancellationToken ct);
    Task<bool> CanDeleteRoomAsync(long roomId, long userId, CancellationToken ct);
}