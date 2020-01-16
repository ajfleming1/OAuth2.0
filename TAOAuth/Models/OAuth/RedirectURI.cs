using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAOAuth.Models.OAuth
{
  public class RedirectURI
  {
    public string OAuthClientId { get; set; }
    public OAuthClient OAuthClient { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string URI { get; set; }
  }
}