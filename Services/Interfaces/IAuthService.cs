using Portifolio.Models.ViewModels;

namespace Portifolio.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Interface responsável em validar credenciais do fluxo de login.
        /// </summary>
        /// <param name="model">Parâmetro que será validade: Email e Senhas do LoginViewModel.</param>
        /// <returns>Retorna true caso as credenciais sejam válidas no banco de dados.</returns>
        Task<bool> AuthAsync(LoginViewModel model);
    }
}