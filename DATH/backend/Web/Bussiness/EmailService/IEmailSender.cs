using MimeKit.Text;

namespace Bussiness.EmailService
{
    public interface IEmailSender
    {
        void SendEmail(EmailMessage message, TextFormat textFormat = TextFormat.Text);
        Task SendEmailAsync(EmailMessage message, TextFormat textFormat = TextFormat.Text);
    }
}
