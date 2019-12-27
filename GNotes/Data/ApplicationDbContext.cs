using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GNotes.Models;
using GNotes.Models.OAuth;

namespace GNotes.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public DbSet<OAuthClient> ClientApplications { get; set; }
    public DbSet<Token> Tokens { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      /* An OAuthClients name is unique among all other OAuthClients */
      builder.Entity<OAuthClient>()
        .HasAlternateKey(x => x.ClientName);

      /* When an AspNet User is deleted, delete their created OAuthClients */
      builder.Entity<OAuthClient>()
        .HasOne(oAuthClient => oAuthClient.Owner)
        .WithMany(applicationUser => applicationUser.UsersOAuthClients)
        .OnDelete(DeleteBehavior.Cascade);

      /* When an OAuthClient is deleted, delete its Rate Limits */
      builder.Entity<OAuthClient>()
        .HasOne(oAuthClient => oAuthClient.RateLimit)
        .WithOne(rateLimit => rateLimit.Client)
        .HasForeignKey<RateLimit>(rateLimit => rateLimit.ClientId)
        .OnDelete(DeleteBehavior.Cascade);

      /* When an OAuthClient is deleted, delete its Subordinate Rate Limit */
      builder.Entity<OAuthClient>()
        .HasOne(oAuthClient => oAuthClient.SubordinateTokenLimits)
        .WithOne(rateLimit => rateLimit.SubordinatedClient)
        .HasForeignKey<RateLimit>(rateLimit => rateLimit.SubordinatedClientId)
        .OnDelete(DeleteBehavior.Cascade);

      /* When a Rate Limit is deleted, delete any Tokens that use this rate limit */
      builder.Entity<RateLimit>()
        .HasOne(rateLimit => rateLimit.Token)
        .WithOne(token => token.RateLimit)
        .OnDelete(DeleteBehavior.Cascade);

      /* When an AspNetUser is deleted, delete their tokens */
      builder.Entity<ApplicationUser>()
        .HasMany(applicationUser => applicationUser.UserClientTokens)
        .WithOne(token => token.User)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      /* When an OAuth Client is deleted, delete any Redirect URIs it used. */
      builder.Entity<RedirectURI>()
        .HasOne(redirectUri => redirectUri.OAuthClient)
        .WithMany(oAuthClient => oAuthClient.RedirectURIs)
        .HasForeignKey(x => x.OAuthClientId)
        .OnDelete(DeleteBehavior.Cascade);

      /* When an OAuth Client is deleted, delete any tokens it issued */
      builder.Entity<OAuthClient>()
        .HasMany(oAuthClient => oAuthClient.UserApplicationTokens)
        .WithOne(token => token.Client)
        .HasForeignKey(x => x.OAuthClientId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}
