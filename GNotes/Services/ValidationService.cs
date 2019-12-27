using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.EntityFrameworkCore;
using GNotes.Data;
using GNotes.Models.OAuth;

namespace GNotes.Services
{
  public class ValidationService
  {
    private readonly ApplicationDbContext _context;

    public ValidationService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<bool> CheckClientIdIsValid(string clientId)
    {
      return await _context.ClientApplications.AnyAsync(c => c.ClientId == clientId);
    }

    public async Task<bool> CheckClientIdAndSecretIsValid(string clientId, string clientSecret)
    {
      if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
      {
        return false;
      }

      // This could be an easy check, but the ASOS maintainer strongly recommends you to use a fixed-time string compare for client secrets.
      // This is trivially available in any .NET Core 2.1 or higher framework, but this is a 2.0 project, so we will leave that part out.
      // If you are on 2.1+, checkout the System.Security.Cryptography.CryptographicOperations.FixedTimeEquals() method,
      // available at https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptographicoperations.fixedtimeequals?view=netcore-2.1
      return await _context.ClientApplications.AnyAsync(x => x.ClientId == clientId && x.ClientSecret == clientSecret);
    }

    public async Task<bool> CheckRedirectUriMatchesClientId(string clientId, string redirectUri)
    {
      if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(redirectUri))
      {
        return false;
      }

      return await _context.ClientApplications
                           .Include(x => x.RedirectURIs)
                           .AnyAsync(oAuthClient => oAuthClient.ClientId == clientId && 
                                             oAuthClient.RedirectURIs.Any(y => y.URI == redirectUri));
    }

    public async Task<bool> CheckRefreshTokenIsValid(string refresh)
    {
      if (string.IsNullOrWhiteSpace(refresh))
      {
        return false;
      }

      return await _context.ClientApplications
        .Include(oAuthClient => oAuthClient.UserApplicationTokens)
        .AnyAsync(oAuthClient => oAuthClient.UserApplicationTokens
        .Any(token => token.TokenType == OpenIdConnectConstants.TokenUsages.RefreshToken && token.Value == refresh));
    }

    public bool CheckScopesAreValid(string scope)
    {
      if (string.IsNullOrWhiteSpace(scope))
      {
        return true; // Unlike the other checks, an empty scope is a valid scope. It just means the application has default permissions.
      }

      var scopes = scope.Split(' ');
      return scopes.All(OAuthScope.NameInScopes);
    }
  }
}