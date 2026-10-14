using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Register;

namespace Portifolio.Controllers
{
    /// <summary>
    /// Coordena o fluxo de cadastro de novas contas.
    /// </summary>
    public class RegisterController : Controller
    {
        private readonly IRegisterService _registerService;
        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        /// <summary>
        /// Rota POST que valida as informações de pré-registro de novas contas.
        /// </summary>
        /// <param name="model">Informações de nova conta a serem validadas.</param>
        /// <returns>Redireciona o usuário para página de confirmação de código se as informações forem válidas.</returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Password = string.Empty;
                TempData["RegisterModel"] = JsonSerializer.Serialize(model);

                TempData["ErrorMessage"] = "Os dados informados estão em um formato inválido.";

                return RedirectToAction("Register", "Login");
            }

            var result = await _registerService.PreRegisterAsync(model);

            if (!result.IsSuccess)
            {
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                TempData["RegisterModel"] = JsonSerializer.Serialize(model);

                TempData["ErrorMessage"] = result.ErrorMessage;

                return RedirectToAction("Register", "Login");
            }

            TempData["RegisterModel"] = model.Email;

            TempData["PermitirAcessoConfirmacao"] = true;

            return RedirectToAction(nameof(ConfirmCode));
        }

        /// <summary>
        /// Rota GET para confirmação de código.
        /// </summary>
        /// <returns>Renderiza a página para verificação do código.</returns>
        [HttpGet]
        public IActionResult ConfirmCode()
        {
            /* Verifica se o email foi redirecionado a partir da página anterior (/Register/Register),
            impedindo acesso direto a URL de confirmação de código. */
            if (TempData["RegisterModel"] == null || TempData["PermitirAcessoConfirmacao"] == null)
            {
                return RedirectToAction("Register", "Login");
            }

            TempData.Keep("RegisterModel");
            return View();
        }

        /// <summary>
        /// Rota POST que processa a validação do código de confirmação enviado pelo usuário.
        /// </summary>
        /// <param name="code">Código de verificação digitado no formulário.</param>
        /// <returns>Redireciona para a página inicial/login em caso de sucesso, ou recarrega a página de 
        /// confirmação exibindo o erro correspondente.</returns>
        [HttpPost]
        public async Task<IActionResult> VerifyCodeAsync(string code)
        {
            string emailString = TempData["RegisterModel"]?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(emailString))
            {
                TempData["ErrorMessage"] = "Sua sessão expirou. Por favor, reinicie o processo de cadastro.";
                return RedirectToAction("Register", "Login");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                TempData.Keep("RegisterModel");
                TempData["PermitirAcessoConfirmacao"] = true;
                TempData["ErrorMessage"] = "Por favor, digite o código de confirmação.";
                
                return RedirectToAction(nameof(ConfirmCode));
            }

            var result = await _registerService.VerifyCodeAsync(emailString, code);

            if (!result.IsSuccess)
            {
                TempData.Keep("RegisterModel");
                TempData["PermitirAcessoConfirmacao"] = true; 

                TempData["ErrorMessage"] = result.ErrorMessage;

                return RedirectToAction(nameof(ConfirmCode));
            }

            TempData["SuccessMessage"] = "Sua conta foi criada e ativada com sucesso! Faça seu login.";

            return RedirectToAction("Index", "Home");
        }
    }
}
