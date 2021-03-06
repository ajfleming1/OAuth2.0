﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuth.Models.OAuth
{
  public class RateLimit
  {
    [Key]
    public int RateLimitId { get; set; } // Primary key for Entity Framework, because this will also be a database object

    public int? Limit { get; set; } // Nullable, so that a limit of 'null' may represent no limit at all.

    public TimeSpan? Window { get; set; } // The timespan of the rolling window. 

    public string ClientId { get; set; }
    public OAuthClient Client { get; set; }

    public static RateLimit DefaultClientLimit =>
      new RateLimit()
      {
        Limit = 10000,
        Window = TimeSpan.FromHours(1),
      };

    public static RateLimit DefaultImplicitLimit =>
      new RateLimit()
      {
        Limit = 150,
        Window = TimeSpan.FromHours(1)
      };

    public static RateLimit DefaultAuthorizationCodeLimit =>
      new RateLimit()
      {
        Limit = 500,
        Window = TimeSpan.FromHours(1)
      };
  }
}