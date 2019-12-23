﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TAOAuth.Data;

namespace TAOAuth.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20191223034425_WithRateLimit")]
    partial class WithRateLimit
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("TAOAuth.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("TAOAuth.Models.OAuth.OAuthClient", b =>
                {
                    b.Property<string>("ClientId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientDescription")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.Property<string>("ClientName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ClientSecret")
                        .IsRequired();

                    b.Property<string>("Id")
                        .IsRequired();

                    b.HasKey("ClientId");

                    b.HasAlternateKey("ClientName");

                    b.HasIndex("Id");

                    b.ToTable("ClientApplications");
                });

            modelBuilder.Entity("TAOAuth.Models.OAuth.RateLimit", b =>
                {
                    b.Property<int>("RateLimitId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId");

                    b.Property<int?>("Limit");

                    b.Property<string>("SubordinatedClientId");

                    b.Property<int?>("TokenId");

                    b.Property<TimeSpan?>("Window");

                    b.HasKey("RateLimitId");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.HasIndex("SubordinatedClientId")
                        .IsUnique();

                    b.HasIndex("TokenId")
                        .IsUnique();

                    b.ToTable("RateLimit");
                });

            modelBuilder.Entity("TAOAuth.Models.OAuth.RedirectURI", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("OAuthClientId");

                    b.Property<string>("URI");

                    b.HasKey("Id");

                    b.HasIndex("OAuthClientId");

                    b.ToTable("RedirectURI");
                });

            modelBuilder.Entity("TAOAuth.Models.OAuth.Token", b =>
                {
                    b.Property<int>("TokenId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GrantType");

                    b.Property<string>("OAuthClientId");

                    b.Property<string>("TokenType");

                    b.Property<string>("UserId");

                    b.Property<string>("Value");

                    b.HasKey("TokenId");

                    b.HasIndex("OAuthClientId");

                    b.HasIndex("UserId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("TAOAuth.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("TAOAuth.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TAOAuth.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("TAOAuth.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TAOAuth.Models.OAuth.OAuthClient", b =>
                {
                    b.HasOne("TAOAuth.Models.ApplicationUser", "Owner")
                        .WithMany("UsersOAuthClients")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TAOAuth.Models.OAuth.RateLimit", b =>
                {
                    b.HasOne("TAOAuth.Models.OAuth.OAuthClient", "Client")
                        .WithOne("RateLimit")
                        .HasForeignKey("TAOAuth.Models.OAuth.RateLimit", "ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TAOAuth.Models.OAuth.OAuthClient", "SubordinatedClient")
                        .WithOne("SubordinateTokenLimits")
                        .HasForeignKey("TAOAuth.Models.OAuth.RateLimit", "SubordinatedClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TAOAuth.Models.OAuth.Token", "Token")
                        .WithOne("RateLimit")
                        .HasForeignKey("TAOAuth.Models.OAuth.RateLimit", "TokenId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TAOAuth.Models.OAuth.RedirectURI", b =>
                {
                    b.HasOne("TAOAuth.Models.OAuth.OAuthClient", "OAuthClient")
                        .WithMany("RedirectURIs")
                        .HasForeignKey("OAuthClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TAOAuth.Models.OAuth.Token", b =>
                {
                    b.HasOne("TAOAuth.Models.OAuth.OAuthClient", "Client")
                        .WithMany("UserApplicationTokens")
                        .HasForeignKey("OAuthClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TAOAuth.Models.ApplicationUser", "User")
                        .WithMany("UserClientTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
