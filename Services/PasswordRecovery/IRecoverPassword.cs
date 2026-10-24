using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;

namespace Portifolio.Services.PasswordRecovery
{
    public interface IRecoverPassword
    {
        /// <summary>
        /// Gera um código de verificação temporário e envia para o e-mail do usuário, caso ele exista no sistema.
        /// </summary>
        /// <param name="model">Modelo contendo o endereço de e-mail do usuário que solicitou a recuperação.</param>
        /// <returns>
        /// Um <see cref="PreRegisterResult"/> indicando se a operação foi bem-sucedida ou se ocorreu alguma falha interna/de envio.
        /// </returns>
        Task<PreRegisterResult> GenerateCodeAndSendEmailAsync(EmailViewModel model);
    }
}