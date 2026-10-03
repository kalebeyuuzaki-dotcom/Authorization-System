using Portifolio.Models.ViewModels;

namespace Portifolio.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Valida duplicidade de E-mail no banco.
        /// </summary>
        /// <param name="email">E-mail que será varificado.</param>
        /// <returns>Retorna true caso já o E-mail já exista no banco.</returns>
        /// Deve retornar uma mensagem genérica para o usuário: "Email e/ou senha incorretos"
        public bool ExistsByEmail(string email); 
    }
}