using Microsoft.Extensions.DependencyInjection;
using OwlPost.Core.Services;
using OwlPost.Core.ServicesContract;

namespace OwlPost.Core;

public static class OwlPostCoreExtensions
{
    public static void AddOwlPostCore(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddSingleton(TimeProvider.System);

    }
}