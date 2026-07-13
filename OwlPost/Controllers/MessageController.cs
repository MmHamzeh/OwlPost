using Microsoft.AspNetCore.Mvc;
using OwlPost.Services;

namespace OwlPost.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessageController(MessagingService messagingService) : ControllerBase
{


    [HttpPost("/Send")]
    public async Task SendMessage(SendMessageDto dto, CancellationToken ct)
    {
        await messagingService.SendMessage(dto, ct);
    }
}
