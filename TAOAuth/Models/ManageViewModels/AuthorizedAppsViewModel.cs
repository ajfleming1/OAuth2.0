using System.Collections.Generic;
using TAOAuth.Models.OAuth;

namespace TAOAuth.Models.ManageViewModels
{
  public class AuthorizedAppsViewModel
  {
    public IList<OAuthClient> AuthorizedApps { get; set; }
  }
}