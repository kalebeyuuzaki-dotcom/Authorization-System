using Portifolio.Models.Domain;
using Portifolio.Models.ViewModels;

namespace Portifolio.Services.Authentication
{
    /// <summary>
    /// Interface responsável por validar credenciais do usuário no fluxo de login.
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Valida se o email de usuário existe no banco de dados, e em seguida 
        /// verifica se a senha informada é igual a senha salva (hash).
        /// </summary>
        /// <param name="model">Credenciais do usuário (email e senha) a serem validados.</param>
        /// <returns>Retorna UserSessionViewModel para criar sessão de login se a validação for bem-sucedida.
        /// Caso contrário, retorna null.</returns>
        Task<UserSessionViewModel?> AuthenticateAsync(LoginViewModel model);
    }
}