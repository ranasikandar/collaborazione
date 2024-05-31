using System.Threading.Tasks;

namespace collaborazione.Utilities.Email
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(
            string fromDisplayName,
            string fromEmailAddress,
            string toName,
            string toEmailAddress,
            string subject,
            string message);
    }
}
