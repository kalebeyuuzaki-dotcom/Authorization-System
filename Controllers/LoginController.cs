using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Authentication;

namespace Portifolio.Controllers
{
    /// <summary>
    /// Controla o fluxo de login do usuário.
    /// </summary>
    public class LoginController : Controller
    {
        private readonly IAuthenticationService _authService;

        public LoginController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Valida as credenciais de login com os registrados no banco de dados.
        /// </summary>
        /// <param name="model">Parâmetros a serem validados.</param>
        /// <returns>Redireciona usuário para 'Panel' se as credenciais de login forem válidas.</returns>
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromForm] LoginViewModel model)
        {
            if (!ModelState.IsValid || !await _authService.AuthenticateAsync(model))
            {
                model.Password = string.Empty;
                TempData["LoginModel"] = JsonSerializer.Serialize(model);
                TempData["ErrorMessage"] = "Usuário e/ou senha incorretos";

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(Panel));
        }

        /// <summary>
        /// Rota GET que redireciona o usuário após sucesso no login. 
        /// </summary>
        /// <returns>Renderiza a página 'Panel'.</returns>
        [HttpGet]
        public IActionResult Panel()
        {
            return View();
        }

        /// <summary>
        /// Carrega a página para Criar Nova Conta.
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}
