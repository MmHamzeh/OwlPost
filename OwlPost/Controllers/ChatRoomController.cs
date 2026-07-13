using Microsoft.AspNetCore.Mvc;
using OwlPost.IoModels.ResponseModels;
using OwlPost.Services;

namespace OwlPost.Controllers;

public class ChatRoomController(ChatRoomService chatRoomService) : ApplicationBaseController
{

    [HttpPost("/Join")]
    public async Task<ApiResponse> JoinRoom(JoinRoomDto dto, CancellationToken ct)
    {
        return await chatRoomService.JoinRoom(dto, ct);
    }


    [HttpPost("/Leave")]
    public async Task<ApiResponse> LeaveRoom(LeaveRoomDto dto, CancellationToken ct)
    {
        return await chatRoomService.LeaveRoom(dto, ct);
    }
}