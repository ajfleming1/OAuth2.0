using System.Threading.Tasks;

namespace OAuth.Services
{
  public interface IEmailSender
  {
    Task SendEmailAsync(string email, string subject, string message);
  }
}