using Microsoft.AspNetCore.SignalR;
using OwlPost.Core.Models;
using OwlPost.Core.ServicesContract;
using OwlPost.SignalR.Hubs;

namespace OwlPost.SignalR.Services;

public class ChatHubService(IHubContext<ChatHub> hubContext) : IChatHubService
{
    public async Task NotifyMessageAdded(ChatMessage entity, CancellationToken ct)
    {
        var signalrGroupId = entity.ChatRoomId.ToString();

        await hubContext.Clients
            .Group(signalrGroupId)
            .SendAsync("message.created", entity 
            , ct);
    }
}