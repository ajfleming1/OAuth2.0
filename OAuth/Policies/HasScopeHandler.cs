using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace OAuth.Policies
{
  public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
  {
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
      IEnumerable<Claim> scopeClaims = context.User.FindAll(claim => claim.Type == "scope" && claim.Issuer == requirement.Issuer);
      // If user does not have the scope claim, get out of here
      var claims = scopeClaims as Claim[] ?? scopeClaims.ToArray();
      if (!claims.Any())
      {
        return Task.CompletedTask;
      }

      // Split the scopes string into an array
      IEnumerable<string> scopes = claims.Select(x => x.Value);
      // Succeed if the scope array contains the required scope
      if (scopes.Any(s => s == requirement.Scope))
      {
        context.Succeed(requirement);
      }
      return Task.CompletedTask;
    }
  }
}