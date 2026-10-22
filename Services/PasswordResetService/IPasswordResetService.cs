using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;

namespace Portifolio.Services.PasswordResetService
{
    public interface IPasswordResetService
    {
        /// <summary>
        /// Salva a nova senha do usuário.
        /// </summary>
        /// <param name="model">Modelo para salvar nova senha, contendo (ex.: E-mail, Nova Senha e Confirmar Nova Senha).</param>
        /// <returns>Retorna um result pattern true, indicando sucesso, ou false com mensagem descritiva de erro.</returns>
        Task<PreRegisterResult> SaveNewPasswordAsync(ResetPasswordViewModel model);
    }
}