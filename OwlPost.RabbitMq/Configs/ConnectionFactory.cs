namespace OwlPost.RabbitMq.Configs;

internal sealed class ConnectionFactory
{
    #region Fields and Ctor

    private IConnection? _connection;

    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;


    internal ConnectionFactory()
    {
        _hostName = "localhost";
        _userName = "guest";
        _password = "guest";
    }

    internal ConnectionFactory(ConnectionOption connectionOption)
    {
        _hostName = connectionOption.HostName;
        _userName = connectionOption.UserName;
        _password = connectionOption.Password;
    }

    #endregion


    public async Task<IConnection> GetRabbitConnection(bool forceRestartConnection = false)
    {
        if (_connection is not null && _connection.IsOpen)
        {
            if (forceRestartConnection == false) 
            return _connection;
            
            await _connection.CloseAsync();
        }

        if (_connection is not null && _connection.IsOpen == false)
            await _connection.DisposeAsync();

        var factory = new RabbitMQ.Client.ConnectionFactory()
        {
            HostName = _hostName,
            UserName = _userName,
            Password = _password
        };

        _connection = await factory.CreateConnectionAsync()
            ?? throw new Exception("Cannot create RabbitMQ Connection");

        return _connection;
    }
}
