namespace OwlPost.RabbitMq.Models;

internal sealed class ConnectionOption
{
    internal required string HostName { get; init; }
    internal required string UserName { get; init; }
    internal required string Password { get; init; }
    internal required int Port { get; init; }
}
