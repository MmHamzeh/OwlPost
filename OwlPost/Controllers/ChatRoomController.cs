using Microsoft.AspNetCore.Mvc;

namespace OwlPost.Controllers;

public class ChatRoomController(ChatRoomService chatRoomService) : ApplicationBaseController
{

    [HttpPost("/Create")]
    [EnableRateLimiting(RateLimitPolicies.Write)]
    public async Task<ApiResponse> CreateRoom(CreateRoomDto dto, CancellationToken ct)
    {
        return await chatRoomService.CreateRoom(dto, ct);
    }

    [HttpPatch("/Edit")]
    [EnableRateLimiting(RateLimitPolicies.Write)]
    public async Task<ApiResponse> EditRoom(EditRoomDto dto, CancellationToken ct)
    {
        return await chatRoomService.EditRoom(dto, ct);
    }

    [HttpDelete("/Delete")]
    [EnableRateLimiting(RateLimitPolicies.Write)]
    public async Task<ApiResponse> DeleteRoom(DeleteRoomDto dto, CancellationToken ct)
    {
        return await chatRoomService.DeleteRoom(dto, ct);
    }


    [HttpPost("/Join")]
    [EnableRateLimiting(RateLimitPolicies.Write)]
    public async Task<ApiResponse> JoinRoom(JoinRoomDto dto, CancellationToken ct)
    {
        return await chatRoomService.JoinRoom(dto, ct);
    }


    [HttpPost("/Leave")]
    [EnableRateLimiting(RateLimitPolicies.Write)]
    public async Task<ApiResponse> LeaveRoom(LeaveRoomDto dto, CancellationToken ct)
    {
        return await chatRoomService.LeaveRoom(dto, ct);
    }
}