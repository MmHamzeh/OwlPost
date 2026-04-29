namespace OwlPost.RabbitMq;

internal static class WireUp
{
    internal static void CreateExchanges()
    {
        MainConfig.ExchangeOptions =
        [
            new ExchangeOption()
            {
                Name = "messaging.exchange"
            }
        ];
    }

    internal static void CreateQueues()
    {
        if (MainConfig.ExchangeOptions is null)
            CreateExchanges();

        MainConfig.QueueOptions =
        [
            new QueueOption(MainConfig.ExchangeOptions!.Single(e => e.Name == "messaging.exchange"))
            {
                Name = "messaging.queue",
                Durable = true,
                AutoDelete = true,
                Exclusive = false,
                Arguments = null,
            }
        ];
    }
}
