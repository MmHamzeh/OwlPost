using System.ComponentModel.DataAnnotations;
using System.Threading.RateLimiting;

namespace OwlPost.RateLimitingConfigs;

public sealed class TokenBucketSettings
{
    public const string SectionName = "RateLimiting:TokenBucket";

    public bool Enabled { get; init; } = true;

    [Range(1, int.MaxValue)]
    public int TokenLimit { get; init; } = 100;

    [Range(1, int.MaxValue)]
    public int TokensPerPeriod { get; init; } = 20;

    public TimeSpan ReplenishmentPeriod { get; init; } =
        TimeSpan.FromSeconds(10);

    [Range(0, int.MaxValue)]
    public int QueueLimit { get; init; }

    public QueueProcessingOrder QueueProcessingOrder { get; init; } =
        QueueProcessingOrder.OldestFirst;

    public bool AutoReplenishment { get; init; } = true;
}