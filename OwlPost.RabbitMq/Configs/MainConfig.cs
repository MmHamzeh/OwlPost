namespace OwlPost.RabbitMq.Configs;

internal static class MainConfig
{
    internal static IConnection? Connection { get; set; }
    internal static IChannel? Channel { get; set; }

}
