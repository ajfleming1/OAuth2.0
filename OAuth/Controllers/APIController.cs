using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OAuth.Attributes;
using OAuth.Data;
using OAuth.Models;

namespace OAuth.Controllers
{
  [Route("/api/v1/")]
  public class APIController : Controller
  {
    private readonly ILogger _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public APIController(ILogger<APIController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _logger = logger;
      _context = context;
      _userManager = userManager;
    }

    // Unauthenticated Methods - available to the public
    [HttpGet("hello")]
    public IActionResult Hello()
    {
      return Ok("Hello");
    }

    // Authenticated Methods - only available to those with a valid Access Token
    // Unscoped Methods - Authenticated methods that do not require any specific Scope
    [RateLimit]
    [HttpGet("clientcount")]
    [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ClientCount()
    {
      return Ok("Client Count Get Request was successful but this endpoint is not yet implemented");
    }

    // Scoped Methods - Authenticated methods that require certain scopes
    [RateLimit]
    [HttpGet("birthdate")]
    [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme, Policy = "user-read-birthdate")]
    public IActionResult GetBirthdate()
    {
      return Ok("Birthdate Get Request was successful but this endpoint is not yet implemented");
    }

    [RateLimit]
    [HttpGet("email")]
    [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme, Policy = "user-read-email")]
    public async Task<IActionResult> GetEmail()
    {
      return Ok("Email Get Request was successful but this endpoint is not yet implemented");
    }

    [RateLimit]
    [HttpPut("birthdate")]
    [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme, Policy = "user-modify-birthdate")]
    public IActionResult ChangeBirthdate(string birthdate)
    {
      return Ok("Birthdate Put successful but this endpoint is not yet implemented");
    }

    [RateLimit]
    [HttpPut("email")]
    [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme, Policy = "user-modify-email")]
    public async Task<IActionResult> ChangeEmail(string email)
    {
      return Ok("Email Put request received, but function is not yet implemented");
    }

    [RateLimit]
    // Dynamic Scope Methods - Authenticated methods that return additional information the more scopes are supplied
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = AspNet.Security.OAuth.Validation.OAuthValidationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Me()
    {
      return Ok("User Profile Get request received, but function is not yet implemented");
    }
  }
}