using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OwlPost.Sql;

public static class OwlPostSqlExtension
{
    public static void AddSqlDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextPool<OwlPostDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Default"));
        });

        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IMessageHistoryRepository, MessageHistoryRepository>();
        services.AddScoped<IChatRoomRepository, ChatRoomRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}