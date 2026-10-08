using Portifolio.Models.ViewModels;

namespace Portifolio.Services.Register
{
    public interface IRegisterService
    {
        /// <summary>
        /// Interface responsável por gerenciar o pré-registro de novas contas.
        /// 
        /// Gera código de confirmação de 6 dígitos caso E-mail informado ainda
        /// não exista no banco de dados e envia .
        ///
        /// Salva as informações de pré-registro no banco de dados para confirmação 
        /// do código gerado e, em seguida, envia o email para o usuário via SMTP.
        /// </summary>
        /// <param name="model">Dados informados pelo usuário.</param>
        /// <returns>Retorna true caso as informações informadas pelo usuário sejam válidas.</returns>
        public Task<bool> PreRegisterAsync(RegisterViewModel model);
    }
}