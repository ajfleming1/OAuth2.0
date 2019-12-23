using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAOAuth.Models.OAuth
{
  public class OAuthClient
  {
    [Key]
    public string ClientId { get; set; }

    [Required]
    public string ClientSecret { get; set; }

    [Required]
    [ForeignKey("Id")]
    public ApplicationUser Owner { get; set; }

    public List<RedirectURI> RedirectURIs { get; set; } = new List<RedirectURI>();

    public List<Token> UserApplicationTokens { get; set; } = new List<Token>();

    /* A Rate limit object for our client - separate from any rate limits applied to the users of this application. */
    public RateLimit RateLimit { get; set; }

    /* A rate limit objects for tokens issued to this client - usually null
     * but if a client has been granted special overrides, the limits specified here will be issued to the tokens, 
     * as opposed to the default grant_type token limits.
     * This allows us to offer specific applications increased overall limits, and increased per-user limits, if so desired. */
    public RateLimit SubordinateTokenLimits { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string ClientName { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(300)]
    public string ClientDescription { get; set; }
  }
}