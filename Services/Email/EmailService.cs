using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Portifolio.Configurations;

namespace Portifolio.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromAddress));

            message.To.Add(new MailboxAddress("", to));

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                var secureSocketOption = SecureSocketOptions.StartTls; // Padrão para porta 587
                if (_smtpSettings.Port == 465)
                {
                    secureSocketOption = SecureSocketOptions.SslOnConnect;
                }

                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, secureSocketOption);

                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);

                await client.SendAsync(message);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
