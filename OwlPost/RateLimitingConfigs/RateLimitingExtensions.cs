using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace OwlPost.RateLimitingConfigs;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddApplicationRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection section =
            configuration.GetRequiredSection(TokenBucketSettings.SectionName);

        services
            .AddOptions<TokenBucketSettings>()
            .Bind(section)
            .ValidateDataAnnotations()
            .Validate(
                settings => settings.ReplenishmentPeriod > TimeSpan.Zero,
                "ReplenishmentPeriod must be greater than zero.")
            .Validate(
                settings => settings.TokensPerPeriod <= settings.TokenLimit,
                "TokensPerPeriod should not exceed TokenLimit.")
            .ValidateOnStart();

        TokenBucketSettings settings =
            section.Get<TokenBucketSettings>()
            ?? throw new InvalidOperationException(
                $"Configuration section '{TokenBucketSettings.SectionName}' is invalid.");

        ValidateSettings(settings);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                HttpContext httpContext = context.HttpContext;

                httpContext.Response.StatusCode =
                    StatusCodes.Status429TooManyRequests;

                int? retryAfterSeconds = null;

                if (context.Lease.TryGetMetadata(
                        MetadataName.RetryAfter,
                        out TimeSpan retryAfter))
                {
                    retryAfterSeconds = Math.Max(
                        1,
                        (int)Math.Ceiling(retryAfter.TotalSeconds));

                    httpContext.Response.Headers.RetryAfter =
                        retryAfterSeconds.Value.ToString(
                            CultureInfo.InvariantCulture);
                }

                ILogger logger = httpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("RateLimiting");

                logger.LogWarning(
                    "Request rejected by rate limiter. " +
                    "Method: {Method}, Path: {Path}, Client: {Client}",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    GetPartitionKey(httpContext));

                await httpContext.Response.WriteAsJsonAsync(
                    new RateLimitErrorResponse
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Error = "TooManyRequests",
                        Message = "The request rate limit has been exceeded.",
                        RetryAfterSeconds = retryAfterSeconds,
                        TraceId = httpContext.TraceIdentifier
                    },
                    cancellationToken);
            };

            options.AddPolicy(
                RateLimitPolicies.ApiTokenBucket,
                httpContext =>
                {
                    string partitionKey = GetPartitionKey(httpContext);

                    return RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey,
                        _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = settings.TokenLimit,
                            TokensPerPeriod = settings.TokensPerPeriod,
                            ReplenishmentPeriod =
                                settings.ReplenishmentPeriod,
                            QueueLimit = settings.QueueLimit,
                            QueueProcessingOrder =
                                settings.QueueProcessingOrder,
                            AutoReplenishment =
                                settings.AutoReplenishment
                        });
                });
        });

        return services;
    }

    private static string GetPartitionKey(HttpContext context)
    {
        // The authenticated user ID is preferred because it cannot be
        // changed as easily as an arbitrary request header.
        string? userId = context.User
            .FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            return $"user:{userId}";
        }

        // Anonymous requests are limited by IP address.
        string ipAddress =
            context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return $"ip:{ipAddress}";
    }

    private static void ValidateSettings(TokenBucketSettings settings)
    {
        if (settings.TokenLimit <= 0)
        {
            throw new InvalidOperationException(
                "TokenLimit must be greater than zero.");
        }

        if (settings.TokensPerPeriod <= 0)
        {
            throw new InvalidOperationException(
                "TokensPerPeriod must be greater than zero.");
        }

        if (settings.ReplenishmentPeriod <= TimeSpan.Zero)
        {
            throw new InvalidOperationException(
                "ReplenishmentPeriod must be greater than zero.");
        }

        if (settings.QueueLimit < 0)
        {
            throw new InvalidOperationException(
                "QueueLimit cannot be negative.");
        }
    }

    private sealed class RateLimitErrorResponse
    {
        public int Status { get; init; }

        public required string Error { get; init; }

        public required string Message { get; init; }

        public int? RetryAfterSeconds { get; init; }

        public required string TraceId { get; init; }
    }
}
