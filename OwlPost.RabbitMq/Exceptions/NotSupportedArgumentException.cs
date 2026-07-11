namespace OwlPost.RabbitMq.Exceptions;

public class NotSupportedArgumentException : MessageBrokerException
{
    public NotSupportedArgumentException() : base()
    {

    }

    private NotSupportedArgumentException(string message) 
        : base(message)
    {

    }

    private NotSupportedArgumentException(string message, MessageBrokerException innerException) 
        : base(message, innerException)
    {
        
    }


    public NotSupportedArgumentException(string requestArgName, Type type) 
        : this($"Type of \"{requestArgName}\" must be \"{type.Name}\"")
    {

    }

    public NotSupportedArgumentException(string requestArgName, Type type, MessageBrokerException innerException)
        : this($"Type of \"{requestArgName}\" must be \"{type.Name}\"", innerException)
    {

    }
}