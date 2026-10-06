using Portifolio.Models.ViewModels;

namespace Portifolio.Services.Interfaces
{
    /// <summary>
    /// Interface responsável por validar credenciais do usuário no fluxo de login.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Verifica se o E-mail do usuário existe no banco de dados.
        /// </summary>
        /// <param name="model">E-mail e Senha a serem verificados.</param>
        /// <returns>Retorna true caso as credenciais sejam válidas no banco de dados.</returns>
        public Task<bool> AuthAsync(LoginViewModel model);
    }
}