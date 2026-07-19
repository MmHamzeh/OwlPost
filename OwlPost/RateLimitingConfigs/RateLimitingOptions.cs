namespace OwlPost.RateLimitingConfigs;

public sealed class RateLimitingOptions
{
    public string? DefaultPolicy { get; init; }

    public Dictionary<string, TokenBucketPolicyOptions> Policies { get; init; }
        = new(StringComparer.OrdinalIgnoreCase);
}