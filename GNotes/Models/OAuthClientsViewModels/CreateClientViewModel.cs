using System.ComponentModel.DataAnnotations;

namespace GNotes.Models.OAuthClientsViewModels
{
  public class CreateClientViewModel
  {
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string ClientName { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(500)]
    public string ClientDescription { get; set; }
  }
}