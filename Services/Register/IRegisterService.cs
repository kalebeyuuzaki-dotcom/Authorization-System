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
        Task<PreRegisterResult> PreRegisterAsync(RegisterViewModel model);

        /// <summary>
        /// Interface responsável por realizar a validação do código de ativação enviado pelo usuário.
        /// 
        /// Executa em ordem cronológica as verificações de: existência do pré-registro, 
        /// bloqueio por excesso de tentativas falhas e tempo de expiração do código.
        /// 
        /// Caso o código seja válido, realiza a persistência atômica movendo os dados temporários 
        /// para a tabela definitiva de usuários do sistema de forma segura.
        /// </summary>
        /// <param name="email">O endereço de e-mail do usuário que está tentando validar a conta.</param>
        /// <param name="code">O código numérico de 6 dígitos enviado por e-mail.</param>
        /// <returns>Retorna um Result Pattern true indicando que a conta foi ativada com sucesso, 
        /// ou false acompanhado de uma mensagem descritiva detalhando o motivo da falha (ex: código incorreto, expirado ou bloqueado).</returns>
        Task<PreRegisterResult> VerifyCodeAsync(string email, string code);
    }
}