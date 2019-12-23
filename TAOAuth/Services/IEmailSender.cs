using System.Threading.Tasks;

namespace TAOAuth.Services
{
  public interface IEmailSender
  {
    Task SendEmailAsync(string email, string subject, string message);
  }
}