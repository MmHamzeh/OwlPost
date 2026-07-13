using OwlPost.IoModels.ResponseModels;

namespace OwlPost.Services;

public class ChatRoomService(
    IMessageBus messageBus,
    ISanitizer sanitizer,
    TimeProvider timeProvider,
    IChatRoomRepository chatRoomRepository,
    IUserService userService)
{
    public async Task<ApiResponse> JoinRoom(JoinRoomDto dto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse> LeaveRoom(LeaveRoomDto dto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}