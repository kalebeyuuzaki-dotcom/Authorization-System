using Portifolio.Models.Domain;
using Portifolio.Models.ViewModels;

namespace Portifolio.Services.Jobs
{
    public interface IRegisterAccountJob
    {
        /// <summary>
        /// Salva requisição de Registro de Conta e dispara e-mail utilizando uma fila Hangfire (Fire-and-Forgot).
        /// </summary>
        /// <param name="email">Email do destinatário.</param>
        /// <param name="subject">Assunto do email.</param>
        /// <param name="body">Corpo HTML do email.</param>
        /// <param name="model">Modelo de requisição de Registro de Conta.</param>
        /// <returns></returns>
        Task TrySaveRequestAndSendEmailAsync(string email, string subject, string body, PreRegisterRequest model);
    }
}