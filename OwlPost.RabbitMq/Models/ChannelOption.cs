namespace OwlPost.RabbitMq.Models;

internal class ChannelOption
{
    internal bool PublisherConfirmationsEnabled { get; set; }
    internal bool PublisherConfirmationTrackingEnabled { get; set; }
    internal TimeSpan? ContinuationTimeout { get; set; }


    internal TimeSpan DefaultContinuationTimeout => TimeSpan.FromSeconds(30);

}
