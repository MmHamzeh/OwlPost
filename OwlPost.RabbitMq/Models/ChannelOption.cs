namespace OwlPost.RabbitMq.Models;

internal sealed class ChannelOption
{
    internal bool PublisherConfirmationsEnabled { get; init; }
    internal bool PublisherConfirmationTrackingEnabled { get; init; }
    internal TimeSpan? ContinuationTimeout { get; init; }
    internal ushort PrefetchCount { get; init; }
    internal uint PrefetchSize { get; init; }

    internal TimeSpan DefaultContinuationTimeout => TimeSpan.FromSeconds(30);

}
