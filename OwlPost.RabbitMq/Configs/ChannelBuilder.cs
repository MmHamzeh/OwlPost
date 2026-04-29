namespace OwlPost.RabbitMq.Configs;

internal class ChannelBuilder
{
    internal async Task<IChannel> GetChannelAsync()
    {
        if (MainConfig.Channel is not null)
            return MainConfig.Channel;

        return await MainConfig.Connection!.CreateChannelAsync();
    }
}
