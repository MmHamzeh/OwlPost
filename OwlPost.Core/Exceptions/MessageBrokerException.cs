namespace OwlPost.Core.Exceptions;

public abstract class MessageBrokerException : Exception
{
    protected MessageBrokerException() : base()
    {

    }

    protected MessageBrokerException(string message) : base(message)
    {

    }   

    protected MessageBrokerException(string message, MessageBrokerException innerException) : base(message, innerException)
    {

    }
}