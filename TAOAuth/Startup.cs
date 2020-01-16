using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TAOAuth.Data;
using TAOAuth.Models;
using TAOAuth.Policies;
using TAOAuth.Providers;
using TAOAuth.Services;

namespace TAOAuth
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

      services.AddIdentity<ApplicationUser, IdentityRole>((x) =>
      {
        x.Password.RequiredLength = 6;
        x.Password.RequiredUniqueChars = 0;
        x.Password.RequireNonAlphanumeric = false;
        x.Password.RequireDigit = false;
        x.Password.RequireLowercase = false;
        x.Password.RequireUppercase = false;
      })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      services.AddAuthentication()
        .AddOAuthValidation()
        .AddOpenIdConnectServer(options =>
        {
          options.UserinfoEndpointPath = "/api/v1/me";
          options.TokenEndpointPath = "/api/v1/token";
          options.AuthorizationEndpointPath = "/authorize/";
          options.UseSlidingExpiration =
            false; // False means that new Refresh tokens aren't issued. Our implementation will be doing a no-expiry refresh, and this is one part of it.
          options.AllowInsecureHttp = false; // ONLY FOR TESTING
          options.AccessTokenLifetime =
            TimeSpan.FromHours(1); // An access token is valid for an hour - after that, a new one must be requested.
          options.RefreshTokenLifetime =
            TimeSpan.FromDays(
              365 * 1000); //NOTE - Later versions of the ASOS library support `TimeSpan?` for these lifetime fields, meaning no expiration. 
          // The version we are using does not, so a long running expiration of one thousand years will suffice.
          options.AuthorizationCodeLifetime = TimeSpan.FromSeconds(60);
          options.IdentityTokenLifetime = options.AccessTokenLifetime;
          options.ProviderType = typeof(OAuthProvider);
        });

      // Add application services.
      services.AddTransient<IEmailSender, EmailSender>();
      services.AddScoped<OAuthProvider>();
      services.AddTransient<ValidationService>();
      services.AddTransient<TokenService>();
      services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
      services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseCookiePolicy();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
