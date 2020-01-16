using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Data;
using OAuth.Models.OAuth;
using OAuth.Services;

namespace OAuth.Providers
{
  public class OAuthProvider : OpenIdConnectServerProvider
  {
    private ValidationService _vService;
    private TokenService _tService;

    #region Authorization Requests
    public override async Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
    {
      _vService = context.HttpContext.RequestServices.GetRequiredService<ValidationService>();
      if (!context.Request.IsAuthorizationCodeFlow() && !context.Request.IsImplicitFlow())
      {
        context.Reject(OpenIdConnectConstants.Errors.UnsupportedResponseType, 
          "Only authorization code, refresh token, and token grant types are accepted by this authorization server.");

        return;
      }

      var clientId = context.ClientId;
      var rdi = context.Request.RedirectUri;
      var state = context.Request.State;
      var scope = context.Request.Scope;
      if (string.IsNullOrWhiteSpace(clientId))
      {
        context.Reject(OpenIdConnectConstants.Errors.InvalidClient, "client_id cannot be empty");
        return;
      }

      if (string.IsNullOrWhiteSpace(rdi))
      {
        context.Reject(OpenIdConnectConstants.Errors.InvalidClient, "redirect_uri cannot be empty");
        return;
      }

      if (!await _vService.CheckClientIdIsValid(clientId))
      {
        context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"The supplied client id does not exist");
        return;
      }

      if (!await _vService.CheckRedirectUriMatchesClientId(clientId, rdi))
      {
        context.Reject(OpenIdConnectConstants.Errors.InvalidClient, "The supplied redirect uri is incorrect");
        return;
      }

      if (!_vService.CheckScopesAreValid(scope))
      {
        context.Reject(OpenIdConnectConstants.Errors.InvalidRequest,"One or all of the supplied scopes are invalid");
        return;
      }

      context.Validate();
    }

    public override async Task ApplyAuthorizationResponse(ApplyAuthorizationResponseContext context)
    {
      if (!string.IsNullOrWhiteSpace(context.Error))
      {
        return;
      }

      _tService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();
      var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
      var claimsUser = context.HttpContext.User;
      // Implicit grant is the only flow that gets their token issued here.
      var access = new Token()
      {
        GrantType = OpenIdConnectConstants.GrantTypes.Implicit,
        TokenType = OpenIdConnectConstants.TokenUsages.AccessToken,
        Value = context.AccessToken,
      };

      OAuthClient client = db.ClientApplications.First(x => x.ClientId == context.Request.ClientId);
      if (client == null)
      {
        return;
      }

      if (client.SubordinateTokenLimits == null)
      {
        access.RateLimit = RateLimit.DefaultImplicitLimit;
      }
      else
      {
        access.RateLimit = client.SubordinateTokenLimits;
      }

      await _tService.WriteNewTokenToDatabase(context.Request.ClientId, access, claimsUser);
    }

    #endregion

    #region Token Requests
    public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
    {
      _vService = context.HttpContext.RequestServices.GetRequiredService<ValidationService>();

      // We only accept "authorization_code", "refresh", "token" for this endpoint.
      if (!context.Request.IsAuthorizationCodeGrantType() &&
          !context.Request.IsRefreshTokenGrantType() &&
          !context.Request.IsClientCredentialsGrantType())
      {
        context.Reject(OpenIdConnectConstants.Errors.UnsupportedGrantType, 
            "Only authorization code, refresh token, and token grant types are accepted by this authorization server.");
      }

      var clientId = context.ClientId;
      var clientSecret = context.ClientSecret;
      var redirectUri = context.Request.RedirectUri;

      // Validating the Authorization Code Token Request
      if (context.Request.IsAuthorizationCodeGrantType())
      {
        if (string.IsNullOrWhiteSpace(clientId))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient, "client_id cannot be empty");
          return;
        }

        if (string.IsNullOrWhiteSpace(clientSecret))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"client_secret cannot be empty");
          return;
        }

        if (string.IsNullOrWhiteSpace(redirectUri))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"redirect_uri cannot be empty");
          return;
        }

        if (!await _vService.CheckClientIdIsValid(clientId))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"The supplied client id was does not exist");
          return;
        }

        if (!await _vService.CheckClientIdAndSecretIsValid(clientId, clientSecret))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"The supplied client secret is invalid");
          return;
        }

        if (!await _vService.CheckRedirectUriMatchesClientId(clientId, redirectUri))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"The supplied redirect uri is incorrect");
          return;
        }

        context.Validate();
        return;
      }

      // Validating the Refresh Code Token Request
      if (context.Request.IsRefreshTokenGrantType())
      {
        clientId = context.Request.ClientId;
        clientSecret = context.Request.ClientSecret;
        var refreshToken = context.Request.RefreshToken;
        if (string.IsNullOrWhiteSpace(clientId))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"client_id cannot be empty");
          return;
        }

        if (string.IsNullOrWhiteSpace(clientSecret))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"client_secret cannot be empty");
          return;
        }

        if (!await _vService.CheckClientIdIsValid(clientId))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"The supplied client id does not exist");
          return;
        }

        if (!await _vService.CheckClientIdAndSecretIsValid(clientId, clientSecret))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"The supplied client secret is invalid");
          return;
        }

        if (!await _vService.CheckRefreshTokenIsValid(refreshToken))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"The supplied refresh token is invalid");
          return;
        }

        context.Validate();
        return;
      }

      // Validating Client Credentials Request, aka, 'token'
      if (context.Request.IsClientCredentialsGrantType())
      {
        clientId = context.ClientId;
        clientSecret = context.ClientSecret;
        if (string.IsNullOrWhiteSpace(clientId))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"client_id cannot be empty");
          return;
        }

        if (string.IsNullOrWhiteSpace(clientSecret))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"client_secret cannot be empty");
          return;
        }

        if (!await _vService.CheckClientIdIsValid(clientId))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"The supplied client id does not exist");
          return;
        }

        if (!await _vService.CheckClientIdAndSecretIsValid(clientId, clientSecret))
        {
          context.Reject(OpenIdConnectConstants.Errors.InvalidClient,"The supplied client secret is invalid");
          return;
        }

        context.Validate();
        return;
      }

      context.Reject(OpenIdConnectConstants.Errors.ServerError,"Could not validate the token request");
    }

    public override Task HandleTokenRequest(HandleTokenRequestContext context)
    {
      AuthenticationTicket ticket;

      // Handling Client Credentials
      if (context.Request.IsClientCredentialsGrantType())
      {
        // If we do not specify any form of Ticket, or ClaimsIdentity, or ClaimsPrincipal, our validation will succeed here but fail later.
        // ASOS needs those to serialize a token, and without any, it fails because there's way to fashion a token properly. Check the ASOS source for more details.
        ticket = TicketCounter.MakeClaimsForClientCredentials(context.Request.ClientId);
        context.Validate(ticket);
        return Task.CompletedTask;
      }
      
      // Handling Authorization Codes
      if (context.Request.IsAuthorizationCodeGrantType() || context.Request.IsRefreshTokenGrantType())
      {
        ticket = context.Ticket;
        if (ticket != null)
        {
          context.Validate(ticket);
          return Task.CompletedTask;
        }

        context.Reject(OpenIdConnectConstants.Errors.InvalidRequest,"User isn't valid");
        return Task.CompletedTask;
      }

      // Catch all error
      context.Reject(OpenIdConnectConstants.Errors.ServerError,"Could not validate the token request");
      return Task.CompletedTask;
    }

    public override async Task ApplyTokenResponse(ApplyTokenResponseContext context)
    {
      if (context.Error != null)
      {
        return;
      }

      _tService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();
      var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
      var client = await dbContext.ClientApplications.FirstOrDefaultAsync(oAuthClient => oAuthClient.ClientId == context.Request.ClientId);
      if (client == null)
      {
        return;
      }

      RateLimit rl = client.SubordinateTokenLimits;

      // Implicit Flow Tokens are not returned from the `Token` group of methods - you can find them in the `Authorize` group.
      if (context.Request.IsClientCredentialsGrantType())
      {
        // The only thing returned from a successful client grant is a single `Token`
        var t = new Token()
        {
          TokenType = OpenIdConnectConstants.TokenUsages.AccessToken,
          GrantType = OpenIdConnectConstants.GrantTypes.ClientCredentials,
          Value = context.Response.AccessToken,
          RateLimit = rl ?? RateLimit.DefaultClientLimit,
        };

        await _tService.WriteNewTokenToDatabase(context.Request.ClientId, t);
      }
      else if (context.Request.IsAuthorizationCodeGrantType())
      {
        var access = new Token()
        {
          TokenType = OpenIdConnectConstants.TokenUsages.AccessToken,
          GrantType = OpenIdConnectConstants.GrantTypes.AuthorizationCode,
          Value = context.Response.AccessToken,
          RateLimit = rl ?? RateLimit.DefaultAuthorizationCodeLimit,
        };

        var refresh = new Token()
        {
          TokenType = OpenIdConnectConstants.TokenUsages.RefreshToken,
          GrantType = OpenIdConnectConstants.GrantTypes.AuthorizationCode,
          Value = context.Response.RefreshToken,
        };

        await _tService.WriteNewTokenToDatabase(context.Request.ClientId, access, context.Ticket.Principal);
        await _tService.WriteNewTokenToDatabase(context.Request.ClientId, refresh, context.Ticket.Principal);
      }
      else if (context.Request.IsRefreshTokenGrantType())
      {
        var access = new Token()
        {
          TokenType = OpenIdConnectConstants.TokenUsages.AccessToken,
          GrantType = OpenIdConnectConstants.GrantTypes.AuthorizationCode,
          Value = context.Response.AccessToken,
          RateLimit = rl ?? RateLimit.DefaultAuthorizationCodeLimit,
        };

        await _tService.WriteNewTokenToDatabase(context.Request.ClientId, access, context.Ticket.Principal);
      }
    }

    #endregion

    public override Task MatchEndpoint(MatchEndpointContext context)
    {
      if (context.Options.AuthorizationEndpointPath.HasValue &&
          context.Request.Path.Value.StartsWith(context.Options.AuthorizationEndpointPath))
      {
        context.MatchAuthorizationEndpoint();
      }
      return Task.CompletedTask;
    }
  }
}