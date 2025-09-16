using Microsoft.Extensions.Options;
using StartupTeam.Shared.Models;
using System.Net;
using System.Net.Mail;

namespace StartupTeam.Shared.Services
{
    public class MailService : IMailService
    {
        private readonly EmailSettings _emailSettings;

        public MailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmail(string emailAddress, string subject, string body)
        {
            var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(_emailSettings.Username),
                To =
                {
                    new MailAddress(emailAddress)
                },
                Body = body,
                IsBodyHtml = true,
                Subject = subject
            };

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
