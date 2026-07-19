using Microsoft.AspNetCore.Mvc;

namespace OwlPost.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting(RateLimitPolicies.Read)]
//[DisableRateLimiting]
public class ApplicationBaseController : ControllerBase
{
}
