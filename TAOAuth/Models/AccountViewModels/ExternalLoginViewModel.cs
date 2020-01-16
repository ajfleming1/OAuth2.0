using System.ComponentModel.DataAnnotations;

namespace TAOAuth.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
