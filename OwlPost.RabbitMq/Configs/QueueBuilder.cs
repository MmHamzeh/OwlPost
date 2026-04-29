namespace OwlPost.RabbitMq.Configs;

internal class QueueBuilder
{

    private QueueDeclareOk? _messagingQueue;

    internal async Task CreateQueueList(List<QueueOption> queueOptionList)
    {
        if (_messagingQueue is not null)
            return;

        foreach (var queueOption in queueOptionList!)
        {
            _messagingQueue = await MainConfig.Channel!.QueueDeclareAsync(
            queue: queueOption.Name,
            durable: queueOption.Durable,
            exclusive: queueOption.Exclusive,
            autoDelete: queueOption.AutoDelete,
            arguments: queueOption.Arguments
            ) ?? throw new Exception($"Failed to create queue '{queueOption.Name}'.");

            Console.WriteLine($"Queue '{queueOption.Name}' created successfully.");

            await MainConfig.Channel!.QueueBindAsync(
                queue: queueOption.Name,
                exchange: queueOption.Exchange.Name,
                routingKey: queueOption.Name
            );

            Console.WriteLine
                ($"Queue '{queueOption.Name}' binded to exchange {queueOption.Exchange.Name} successfully.");

        }
    }

}
