namespace Portifolio.Services.Jobs
{
    public interface IPasswordRecoveryJob
    {
        /// <summary>
        /// Envia e-mail assíncrono utilizando uma fila Hangfire (Fire-and-Forget).
        /// </summary>
        /// <param name="email">E-mail do destinatário.</param>
        /// <param name="subject">Assunto do email.</param>
        /// <param name="body">Corpo HTML do email.</param>
        /// <param name="code">Código de verificação.</param>
        /// <returns>Retorna sucesso em caso do email ser enviado, senão, retorna false.</returns>
        Task SendEmailAndSaveRequestAsync(string email, string subject, string body, string code);
    }
}