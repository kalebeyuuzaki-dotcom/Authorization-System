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
        private const string RegisterModelKey = "RegisterModel";
        private const string ErrorMessageKey = "ErrorMessage";
        private const string AllowAccessConfirmationKey = "AllowAccessConfirmation";
        private readonly IRegisterService _registerService;
        private readonly IConfirmationCodeService _confirmationCodeService;

        public RegisterController(IRegisterService registerService, IConfirmationCodeService confirmationCodeService)
        {
            _registerService = registerService;
            _confirmationCodeService = confirmationCodeService;
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
                TempData[RegisterModelKey] = JsonSerializer.Serialize(model);
                TempData[ErrorMessageKey] = "Os dados informados estão em um formato inválido. Por favor, verifique as informações.";

                return RedirectToAction("Register", "Login");
            }

            var result = await _registerService.PreRegisterAsync(model);

            if (!result.IsSuccess)
            {
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                TempData[RegisterModelKey] = JsonSerializer.Serialize(model);
                TempData[ErrorMessageKey] = result.ErrorMessage;

                return RedirectToAction("Register", "Login");
            }

            TempData[RegisterModelKey] = model.Email;
            TempData[AllowAccessConfirmationKey] = true;

            return RedirectToAction(nameof(ConfirmCode));
        }

        /// <summary>
        /// Rota GET para confirmação de código.
        /// </summary>
        /// <returns>Renderiza a página para verificação do código.</returns>
        [HttpGet]
        public IActionResult ConfirmCode()
        {
            /* Verifica se quem está acessando esse endpoint foi redirecionado pela action Register, 
            impedindo acesso direto a essa URL (/Register/ConfirmCode). */
            if (TempData[RegisterModelKey] == null || TempData[AllowAccessConfirmationKey] == null)
            {
                return RedirectToAction("Register", "Login");
            }

            TempData.Keep(RegisterModelKey);

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
            string emailString = TempData[RegisterModelKey]?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(emailString))
            {
                TempData[ErrorMessageKey] = "Sua sessão expirou. Por favor, reinicie o processo de cadastro.";

                return RedirectToAction("Register", "Login");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                TempData.Keep(RegisterModelKey);
                TempData[AllowAccessConfirmationKey] = true;
                TempData[ErrorMessageKey] = "Por favor, digite o código de confirmação.";

                return RedirectToAction(nameof(ConfirmCode));
            }

            var result = await _confirmationCodeService.VerifyCodeAsync(emailString, code);

            if (!result.IsSuccess)
            {
                TempData.Keep(RegisterModelKey);
                TempData[AllowAccessConfirmationKey] = true;
                TempData[ErrorMessageKey] = result.ErrorMessage;

                return RedirectToAction(nameof(ConfirmCode));
            }

            TempData["SuccessMessage"] = "Sua conta foi criada e ativada com sucesso! Faça seu login.";

            return RedirectToAction("Index", "Home");
        }
    }
}
