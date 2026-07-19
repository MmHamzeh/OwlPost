using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace OwlPost.RateLimitingConfigs;


public static class RateLimitingExtensions
{
    public static IServiceCollection AddApplicationRateLimiting(this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("RateLimiting");

        services.AddOptions<RateLimitingOptions>()
            .Bind(section)
            .ValidateOnStart();

        var options = section.Get<RateLimitingOptions>()
                      ?? throw new InvalidOperationException("RateLimiting section is invalid.");

        Validate(options);

        string defaultPolicyName = ResolveDefaultPolicyName(options);

        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds))
                            .ToString(CultureInfo.InvariantCulture);
                }

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    status = 429,
                    error = "TooManyRequests",
                    message = "The request rate limit has been exceeded.",
                    traceId = context.HttpContext.TraceIdentifier
                }, cancellationToken);
            };

            foreach (var policy in options.Policies)
            {
                string policyName = policy.Key;
                TokenBucketPolicyOptions policyOptions = policy.Value;

                rateLimiterOptions.AddPolicy(policyName, httpContext =>
                {
                    string partitionKey = GetPartitionKey(httpContext);

                    return RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey,
                        _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = policyOptions.TokenLimit,
                            TokensPerPeriod = policyOptions.TokensPerPeriod,
                            ReplenishmentPeriod = policyOptions.ReplenishmentPeriod,
                            QueueLimit = policyOptions.QueueLimit,
                            QueueProcessingOrder = policyOptions.QueueProcessingOrder,
                            AutoReplenishment = policyOptions.AutoReplenishment
                        });
                });
            }

            // Default policy: if not explicitly chosen, use the one with the biggest bucket.
            rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var chosen = options.Policies[defaultPolicyName];

                string partitionKey = GetPartitionKey(context);

                return RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = chosen.TokenLimit,
                        TokensPerPeriod = chosen.TokensPerPeriod,
                        ReplenishmentPeriod = chosen.ReplenishmentPeriod,
                        QueueLimit = chosen.QueueLimit,
                        QueueProcessingOrder = chosen.QueueProcessingOrder,
                        AutoReplenishment = chosen.AutoReplenishment
                    });
            });
        });

        return services;
    }

    private static string ResolveDefaultPolicyName(RateLimitingOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.DefaultPolicy) &&
            options.Policies.ContainsKey(options.DefaultPolicy))
        {
            return options.DefaultPolicy;
        }

        // Default fallback: pick the policy with the biggest bucket.
        // TokenLimit is the main signal; if tied, use TokensPerPeriod.
        return options.Policies
            .OrderByDescending(p => p.Value.TokenLimit)
            .ThenByDescending(p => p.Value.TokensPerPeriod)
            .First().Key;
    }

    private static string GetPartitionKey(HttpContext context)
    {
        var userId = context.User?.Identity?.Name;

        if (!string.IsNullOrWhiteSpace(userId))
            return $"user:{userId}";

        return $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";
    }

    private static void Validate(RateLimitingOptions options)
    {
        if (options.Policies.Count == 0)
            throw new InvalidOperationException("At least one rate limit policy must be defined.");

        foreach (var (name, policy) in options.Policies)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Policy name cannot be empty.");

            if (policy.TokenLimit <= 0)
                throw new InvalidOperationException($"Policy '{name}': TokenLimit must be > 0.");

            if (policy.TokensPerPeriod <= 0)
                throw new InvalidOperationException($"Policy '{name}': TokensPerPeriod must be > 0.");

            if (policy.ReplenishmentPeriod <= TimeSpan.Zero)
                throw new InvalidOperationException($"Policy '{name}': ReplenishmentPeriod must be > 0.");

            if (policy.QueueLimit < 0)
                throw new InvalidOperationException($"Policy '{name}': QueueLimit cannot be negative.");
        }
    }
}
