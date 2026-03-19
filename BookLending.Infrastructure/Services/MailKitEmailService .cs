using BookLending.Application.Abstractions;
using BookLending.Application.Setting;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BookLending.Infrastructure.Services
{
    public class MailKitEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public MailKitEmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendOverdueReminderAsync(string toEmail, string toName, string bookTitle, DateTimeOffset dueDate)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(new MailboxAddress(toName, toEmail));
            email.Subject = "Overdue Book Reminder";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Hello {toName},\n\nYour book '{bookTitle}' was due on {dueDate:dd/MM/yyyy}.\nPlease return it as soon as possible.\n\nThank you!"
            };

            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}