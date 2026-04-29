namespace OwlPost.RabbitMq.Configs;

internal class ExchangeBuilder
{

    internal async Task CreateMessagingExchange(List<ExchangeOption> exchangeOptionList)
    {

        foreach (var exchangeOption in exchangeOptionList)
        {
            string exchangeType = exchangeOption.ExchangeTypeEnm switch
            {
                ExchangeTypeEnm.Direct => ExchangeType.Direct,
                ExchangeTypeEnm.Fanout => ExchangeType.Fanout,
                ExchangeTypeEnm.Headers => ExchangeType.Headers,
                ExchangeTypeEnm.Topic => ExchangeType.Topic,
                _ => string.Empty
            };

            await MainConfig.Channel!.ExchangeDeclareAsync(
            exchange: exchangeOption.Name,
            type: exchangeType,
            durable: exchangeOption.Durable,
            autoDelete: exchangeOption.AutoDelete,
            arguments: exchangeOption.Arguments
        );

            Console.WriteLine($"\"{exchangeOption.Name}\" created.");

        }

    }
}
