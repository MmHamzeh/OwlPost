using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OwlPost.RateLimitingConfigs;

namespace OwlPost.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting(RateLimitPolicies.ApiTokenBucket)]
//[DisableRateLimiting]
public class ApplicationBaseController : ControllerBase
{
}
