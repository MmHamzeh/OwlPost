namespace OwlPost.Services;

public class ChatRoomService(
    IMessageBus messageBus,
    ISanitizer sanitizer,
    TimeProvider timeProvider,
    IUserService userService)
{
    private readonly DateTimeOffset _now = timeProvider.GetUtcNow();

    public async Task<ApiResponse> JoinRoom(JoinRoomDto dto, CancellationToken ct)
    {
        var groupingKey = dto.RoomId.ToString();
        var req = new MessageBusJoinRoomRequest(_now.DateTime, userService.UserPublicId, groupingKey, dto.RoomId);
        await messageBus.JoinRoom(req, ct);
        return new ApiResponse();
    }

    public async Task<ApiResponse> LeaveRoom(LeaveRoomDto dto, CancellationToken ct)
    {
        var groupingKey = dto.RoomId.ToString();
        var req = new MessageBusLeaveRoomRequest(_now.DateTime, userService.UserPublicId, groupingKey, dto.RoomId);
        await messageBus.LeaveRoom(req, ct);
        return new ApiResponse();
    }

    public async Task<ApiResponse> CreateRoom(CreateRoomDto dto, CancellationToken ct)
    {
        const string groupingKey = "Create_Room";
        var sanitizedName = sanitizer.Sanitize(dto.Name);
        var sanitizedDescription = sanitizer.Sanitize(dto.Description);

        var req = 
            new MessageBusCreateRoomRequest(_now.DateTime, userService.UserPublicId, groupingKey, sanitizedName, sanitizedDescription);

        await messageBus.CreateRoom(req, ct);
        return new ApiResponse();
    }

    public async Task<ApiResponse> EditRoom(EditRoomDto dto, CancellationToken ct)
    {
        var groupingKey = dto.RoomId.ToString();
        var sanitizedName = sanitizer.Sanitize(dto.Name);
        var sanitizedDescription = sanitizer.Sanitize(dto.Description);

        var req = 
            new MessageBusEditRoomRequest(_now.DateTime, userService.UserPublicId, groupingKey, dto.RoomId, sanitizedName, sanitizedDescription);

        await messageBus.EditRoom(req, ct);
        return new ApiResponse();
    }

    public async Task<ApiResponse> DeleteRoom(DeleteRoomDto dto, CancellationToken ct)
    {
        var groupingKey = dto.RoomId.ToString();
        var req = new MessageBusDeleteRoomRequest(_now.DateTime, userService.UserPublicId, groupingKey, dto.RoomId);

        await messageBus.DeleteRoom(req, ct);
        return new ApiResponse();
    }
}