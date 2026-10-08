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

        // O ASP.NET Core injeta automaticamente as configurações do .env mapeadas via IOptions
        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();

            // Configura o Remetente (Quem envia)
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromAddress));

            // Configura o Destinatário (Quem recebe)
            message.To.Add(new MailboxAddress("", to));

            // Configura o Assunto
            message.Subject = subject;

            // Configura o Corpo do e-mail como HTML
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            // Conecta ao servidor SMTP e envia de forma segura
            using var client = new SmtpClient();
            try
            {
                // Determina o tipo de criptografia com base na porta ou string
                var secureSocketOption = SecureSocketOptions.StartTls; // Padrão para porta 587
                if (_smtpSettings.Port == 465)
                {
                    secureSocketOption = SecureSocketOptions.SslOnConnect;
                }

                // 1. Conecta ao servidor do Gmail
                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, secureSocketOption);

                // 2. Autentica com suas credenciais do .env
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);

                // 3. Realiza o envio
                await client.SendAsync(message);
            }
            finally
            {
                // Garante que a desconexão ocorra mesmo em caso de erro posterior
                await client.DisconnectAsync(true);
            }
        }
    }
}
