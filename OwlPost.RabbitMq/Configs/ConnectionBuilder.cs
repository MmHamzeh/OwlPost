namespace OwlPost.RabbitMq.Configs;

internal sealed class ConnectionBuilder
{
    internal async Task<IConnection> GetRabbitConnection(ConnectionOption connectionOption, bool forceRestartConnection = false)
    {
        if (MainConfig.Connection is not null && MainConfig.Connection.IsOpen)
        {
            if (forceRestartConnection == false) 
            return MainConfig.Connection;
            
            await MainConfig.Connection.CloseAsync();
        }

        if (MainConfig.Connection is not null && MainConfig.Connection.IsOpen == false)
            await MainConfig.Connection.DisposeAsync();

        var factory = new ConnectionFactory()
        {
            HostName = connectionOption.HostName,
            UserName = connectionOption.UserName,
            Password = connectionOption.Password
        };

        return await factory.CreateConnectionAsync()
            ?? throw new Exception("Cannot create RabbitMQ Connection");

    }
}
