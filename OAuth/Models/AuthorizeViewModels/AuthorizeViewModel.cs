﻿namespace OAuth.Models.AuthorizeViewModels
{
  public class AuthorizeViewModel
  {
    public string ClientName { get; internal set; }
    public string ClientId { get; internal set; }
    public string ClientDescription { get; internal set; }
    public string ResponseType { get; internal set; }
    public string RedirectUri { get; internal set; }
    public string[] Scopes { get; internal set; } = new string[0];
    public string State { get; internal set; }
    public string CodeChallenge { get; set; }
    public string CodeChallengeMethod { get; set; }
  }
}