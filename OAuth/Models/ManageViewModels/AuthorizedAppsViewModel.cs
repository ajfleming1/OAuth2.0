using System.Collections.Generic;
using OAuth.Models.OAuth;

namespace OAuth.Models.ManageViewModels
{
  public class AuthorizedAppsViewModel
  {
    public IList<OAuthClient> AuthorizedApps { get; set; }
  }
}