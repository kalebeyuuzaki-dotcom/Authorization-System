using Portifolio.Models.Result;

namespace Portifolio.Services.Register
{
    /// <summary>
    /// Interface responsável por realizar a validação do código de ativação.
    /// </summary>
    public interface IConfirmationCodeService
    {
        /// </summary>
        /// Executa em ordem cronológica as verificações de: existência do pré-registro, 
        /// bloqueio por excesso de tentativas falhas e tempo de expiração do código.
        /// </summary>
        /// <param name="email">O endereço de e-mail do usuário que está tentando validar a conta.</param>
        /// <param name="code">O código numérico de 6 dígitos enviado por e-mail.</param>
        /// <returns>Retorna um Result Pattern true indicando que a conta foi ativada com sucesso, 
        /// ou false acompanhado de uma mensagem descritiva detalhando o motivo da falha (ex: código incorreto, expirado ou bloqueado).</returns>
        Task<PreRegisterResult> VerifyCodeAsync(string email, string code);
    }
}