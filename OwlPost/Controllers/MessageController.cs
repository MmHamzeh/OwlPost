using Microsoft.AspNetCore.Mvc;
using OwlPost.IoModels.ResponseModels;
using OwlPost.Services;

namespace OwlPost.Controllers;

public class MessageController(MessagingService messagingService) : ApplicationBaseController
{
    [HttpPost("/Send")]
    public async Task<ApiResponse> SendMessage(SendMessageDto dto, CancellationToken ct)
    {
        return await messagingService.SendMessage(dto, ct);
    }

    [HttpPatch("Edit")]
    public async Task<ApiResponse> EditMessage(EditMessageDto dto, CancellationToken ct)
    {
        return await messagingService.EditMessage(dto, ct);
    }

    [HttpDelete("Delete")]
    public async Task<ApiResponse> DeleteMessage(DeleteMessageDto dto, CancellationToken ct)
    {
        return await messagingService.DeleteMessage(dto, ct);
    }

}