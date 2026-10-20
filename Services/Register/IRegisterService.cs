using Portifolio.Models.ViewModels;
using Portifolio.Models.Result;

namespace Portifolio.Services.Register
{
    public interface IRegisterService
    {
        /// <summary>
        /// Interface responsável por gerenciar o pré-registro de novas contas.
        /// 
        /// Gera código de confirmação de 6 dígitos caso E-mail informado ainda
        /// não exista no banco de dados.
        ///
        /// Salva as informações de pré-registro no banco de dados para confirmação 
        /// do código gerado e, em seguida, envia o email para o usuário via SMTP.
        /// </summary>
        /// <param name="model">Dados informados pelo usuário.</param>
        /// <returns>Retorna um Result Pattern true indicando que o pré-registro foi salvo no banco, 
        /// ou false com mensagem de erro genérica (ex: usuário e/ou senha inválidos, erro interno).</returns>
        Task<PreRegisterResult> GenerateCodeAndSentEmailAsync(RegisterViewModel model);
    }
}