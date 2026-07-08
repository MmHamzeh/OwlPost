namespace OwlPost.RabbitMq.QueueModels;

public class EnqueueResponse : IDto<Guid>, IMessageBusResponse
{
    protected override void PrepareDto(Guid currentUserId, TimeProvider timeProvider)
        => throw new NotSupportedException();

    public override Guid? PublicId
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
}