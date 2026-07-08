using OwlPost.RabbitMq;
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
        builder.Services.AddRabbitMq(builder.Configuration);
        builder.Services.AddSqlDatabase(builder.Configuration);


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
