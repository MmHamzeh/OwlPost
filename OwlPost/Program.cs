using OwlPost.RabbitMq;
using OwlPost.RateLimitingConfigs;
using OwlPost.Sanitizer;
using OwlPost.Serializer;
using OwlPost.SignalR;
using OwlPost.SignalR.Hubs;
using OwlPost.Sql;

namespace OwlPost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddOwlPostCore();
        builder.Services.AddOwlPostSanitizer();
        builder.Services.AddOwlPostSerializer();
        builder.Services.AddRabbitMq(builder.Configuration);
        builder.Services.AddSqlDatabase(builder.Configuration);
        builder.Services.AddOwlPostSignalR();

        //TODO: create a new project for Authentication, Authorization
        builder.Services.AddScoped<IUserService, OwlPost.Services.UserService>();

        builder.Services.AddScoped<MessagingService>();
        builder.Services.AddScoped<ChatRoomService>();
        builder.Services.AddScoped<IConsumedMessageProcessor, ConsumedMessageProcessor>();
        builder.Services.AddScoped<IMessagingPermissionService, MessagingPermissionService>();
        builder.Services.AddScoped<IRoomPermissionService, RoomPermissionService>();

        builder.Services.AddApplicationRateLimiting(builder.Configuration);


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseRateLimiter();

        app.MapControllers()
            //.RequireRateLimiting(RateLimitPolicies.ApiTokenBucket)
            ;

        app.MapHub<ChatHub>("/hubs/chat");


        app.Run();
    }
}
