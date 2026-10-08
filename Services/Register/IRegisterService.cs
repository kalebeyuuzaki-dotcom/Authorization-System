using Portifolio.Models.ViewModels;

namespace Portifolio.Services.Register
{
    public interface IRegisterService
    {
        /// <summary>
        /// Interface responsável por gerenciar o pré-registro de novas contas.
        /// 
        /// Gera código de confirmação de 6 dígitos caso E-mail informado ainda
        /// não exista no banco de dados de usuários cadastrados.
        ///
        /// Salva as informações de pré-registro no banco de dados para confirmação do código gerado.
        /// </summary>
        /// <param name="model">Dados da model a serem tratados.</param>
        /// <returns>Retorna true caso as informações informadas pelo usuário sejam válidas.</returns>
        public Task<bool> PreRegisterAsync(RegisterViewModel model);
    }
}