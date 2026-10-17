using Portifolio.Models.Result;

namespace Portifolio.Services.PasswordRecovery
{
    /// <summary>
    /// Interface responsável por validar código de recuperação de senha.
    /// </summary>
    public interface IConfirmCodePasswordRecoverService
    {
        /// <summary>
        /// Executa em ordem cronológica as verificações de: existência do pré-registro, 
        /// bloqueio por excesso de tentativas falhas e tempo de expiração do código.
        /// </summary>
        /// <param name="email">O endereço de e-mail do usuário que está tentando recuperar a senha.</param>
        /// <param name="code">O código numérico de 6 dígitos enviado por e-mail.</param>
        /// <returns>etorna um Result Pattern true indicando sucesso na validação do código, 
        /// ou false acompanhado de uma mensagem descritiva detalhando o motivo da falha (ex: código incorreto, expirado ou bloqueado).</returns>
        Task<PreRegisterResult> ValidateCodePasswordRecoverAsync(string email, string code);
    }
}