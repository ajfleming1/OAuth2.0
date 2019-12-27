using System.Threading.Tasks;

namespace GNotes.Services
{
  public interface IEmailSender
  {
    Task SendEmailAsync(string email, string subject, string message);
  }
}