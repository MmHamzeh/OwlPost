using Microsoft.Extensions.DependencyInjection;

namespace OwlPost.Core;

public static class OwlPostCoreExtensions
{
    public static void AddOwlPostCore(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

    }
}