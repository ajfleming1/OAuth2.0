using System.Collections.Generic;
using GNotes.Models.OAuth;

namespace GNotes.Models.ManageViewModels
{
  public class AuthorizedAppsViewModel
  {
    public IList<OAuthClient> AuthorizedApps { get; set; }
  }
}