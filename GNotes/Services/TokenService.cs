using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GNotes.Data;
using GNotes.Models;
using GNotes.Models.OAuth;

namespace GNotes.Services
{
  public class TokenService
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    public async Task WriteNewTokenToDatabase(string clientId, Token token, ClaimsPrincipal user = null)
    {
      if (string.IsNullOrWhiteSpace(clientId) || token == null || string.IsNullOrWhiteSpace(token.GrantType) || string.IsNullOrWhiteSpace(token.Value))
      {
        return;
      }

      var client = await _context.ClientApplications
        .Include(oAuthClient => oAuthClient.Owner)
        .Include(x => x.UserApplicationTokens)
        .Where(x => x.ClientId == clientId).FirstOrDefaultAsync();

      if (client == null)
      {
        return;
      }

      // Handling Client Credentials
      if (token.GrantType == OpenIdConnectConstants.GrantTypes.ClientCredentials)
      {
        var oldClientCredentialTokens = client.UserApplicationTokens.Where(x => x.GrantType == OpenIdConnectConstants.GrantTypes.ClientCredentials).ToList();
        foreach (var old in oldClientCredentialTokens)
        {
          _context.Entry(old).State = EntityState.Deleted;
          client.UserApplicationTokens.Remove(old);
        }

        client.UserApplicationTokens.Add(token);
        _context.Update(client);
        await _context.SaveChangesAsync();
      }

      // Handling the other flows
      else if (token.GrantType == OpenIdConnectConstants.GrantTypes.Implicit || token.GrantType == OpenIdConnectConstants.GrantTypes.AuthorizationCode || token.GrantType == OpenIdConnectConstants.GrantTypes.RefreshToken)
      {
        if (user == null)
        {
          return;
        }

        var au = await _userManager.GetUserAsync(user);
        if (au == null)
        {
          return;
        }

        // These tokens also require association to a specific user
        var oldTokensForGrantType = client.UserApplicationTokens.Where(userToken => userToken.GrantType == token.GrantType && userToken.TokenType == token.TokenType).Intersect(au.UserClientTokens).ToList();
        foreach (var old in oldTokensForGrantType)
        {
          _context.Entry(old).State = EntityState.Deleted;
          client.UserApplicationTokens.Remove(old);
          au.UserClientTokens.Remove(old);
        }

        client.UserApplicationTokens.Add(token);
        au.UserClientTokens.Add(token);
        _context.ClientApplications.Update(client);
        _context.Users.Update(au);

        await _context.SaveChangesAsync();
      }
    }
  }
}