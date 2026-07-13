using Microsoft.Extensions.DependencyInjection;
using OwlPost.Core.ServicesContract;

namespace OwlPost.Sanitizer;

public static class OwlPostSanitizerExtension
{
    public static void AddOwlPostSanitizer(this IServiceCollection services)
    {
        services.AddSingleton<ISanitizer, Sanitizer>();
    }
}