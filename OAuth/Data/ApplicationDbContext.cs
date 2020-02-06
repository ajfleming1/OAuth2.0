using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OAuth.Models;
using OAuth.Models.OAuth;

namespace OAuth.Data
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
