namespace OwlPost.Core.PermissionServicesContract;

public interface IMessagingPermissionService
{
    Task<bool> CanSendMessageAsync(long userId, long roomId, CancellationToken ct);

    Task<bool> CanEditMessageAsync(long messageId, long userId, long roomId, CancellationToken ct);

    Task<bool> CanDeleteMessageAsync(long messageId, long userId, long roomId, CancellationToken ct);
}