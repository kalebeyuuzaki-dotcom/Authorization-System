namespace Portifolio.Services.Email
{
    public interface IEmailService
    {
        /// <summary>
        /// Envia um e-mail assíncrono formatado em HTML.
        /// </summary>
        /// <param name="to">E-mail do destinatário</param>
        /// <param name="subject">Assunto do e-mail</param>
        /// <param name="body">Conteúdo do e-mail (aceita tags HTML)</param>
        Task SendEmailAsync(string to, string subject, string body);
    }
}
