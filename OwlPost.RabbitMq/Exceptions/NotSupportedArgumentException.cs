namespace OwlPost.RabbitMq.Exceptions;

public class NotSupportedArgumentException : MessageBrokerException
{
    public NotSupportedArgumentException() : base()
    {

    }

    public NotSupportedArgumentException(string message) : base(message)
    {

    }

    public NotSupportedArgumentException(string message, MessageBrokerException innerException) : base(message, innerException)
    {
        
    }
}