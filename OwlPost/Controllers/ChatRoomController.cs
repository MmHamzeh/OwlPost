using Microsoft.AspNetCore.Mvc;

namespace OwlPost.Controllers;

public class ChatRoomController(ChatRoomService chatRoomService) : ApplicationBaseController
{

    [HttpPost("/Create")]
    public async Task<ApiResponse> CreateRoom(CreateRoomDto dto, CancellationToken ct)
    {
        return await chatRoomService.CreateRoom(dto, ct);
    }

    [HttpPatch("/Edit")]
    public async Task<ApiResponse> EditRoom(EditRoomDto dto, CancellationToken ct)
    {
        return await chatRoomService.EditRoom(dto, ct);
    }

    [HttpDelete("/Delete")]
    public async Task<ApiResponse> DeleteRoom(DeleteRoomDto dto, CancellationToken ct)
    {
        return await chatRoomService.DeleteRoom(dto, ct);
    }


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