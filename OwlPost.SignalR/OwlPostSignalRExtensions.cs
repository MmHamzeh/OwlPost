using Microsoft.Extensions.DependencyInjection;
using OwlPost.Core.ServicesContract;
using OwlPost.SignalR.Services;

namespace OwlPost.SignalR;

public static class OwlPostSignalRExtensions
{
    public static void AddOwlPostSignalR(this IServiceCollection services)
    {
        services.AddScoped<IChatHubService, ChatHubService>();
    }
}
