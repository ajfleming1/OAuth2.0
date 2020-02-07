using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OAuth.Data;
using OAuth.Models;
using OAuth.Policies;
using OAuth.Providers;
using OAuth.Services;

namespace OAuth
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(ILogger<Startup> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("OAuthConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>((options) =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
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
                    options.UseSlidingExpiration = false;
                    options.AllowInsecureHttp = true;
                    options.AccessTokenLifetime = TimeSpan.FromHours(1);
                    options.RefreshTokenLifetime =
                      TimeSpan.FromDays(365 * 1000);
                    options.AuthorizationCodeLifetime = TimeSpan.FromSeconds(60);
                    options.IdentityTokenLifetime = options.AccessTokenLifetime;
                    options.ProviderType = typeof(OAuthProvider);

                    // Register the HSM signing key.
                    var keyPath = Configuration.GetSection("KeyVault")["KeyPath"];
                    var keyVaultEndpoint = Configuration.GetSection("KeyVault")["KeyVaultEndpoint"];
                    options.SigningCredentials.AddKey(KeyVaultHelper.GetSigningKey(keyVaultEndpoint, keyPath));
                });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<OAuthProvider>();
            services.AddTransient<ValidationService>();
            services.AddTransient<TokenService>();
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddMvc();
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(next => context =>
            {
                var scheme = context.Request.Scheme;
                _logger.LogInformation("Scheme - " + scheme);
                return next(context);
            });

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
