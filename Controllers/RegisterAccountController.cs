using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Register;

namespace Portifolio.Controllers
{
    /// <summary>
    /// Coordena o fluxo de cadastro de novas contas.
    /// </summary>
    public class RegisterAccountController : Controller
    {
        private const string RegisterModelKey = "RegisterModel";
        private const string ErrorMessageKey = "ErrorMessage";
        private const string AllowAccessConfirmationKey = "AllowAccessConfirmation";
        private const string InvalidDataMessage = "Os dados informados estão em um formato inválido. Por favor, verifique as informações.";
        private const string SuccessMessage = "Sua conta foi criada e ativada com sucesso! Faça seu login.";
        private const string SessionExpiredMessage = "Sua sessão expirou. Por favor, reinicie o processo de cadastro.";

        private readonly IRegisterService _registerService;
        private readonly IConfirmationCodeService _confirmationCodeService;

        public RegisterAccountController(IRegisterService registerService, IConfirmationCodeService confirmationCodeService)
        {
            _registerService = registerService;
            _confirmationCodeService = confirmationCodeService;
        }
        
        /// <summary>
        /// Rota GET que carrega a página para criar nova conta.
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Rota POST que valida as informações de pré-registro de novas contas.
        /// </summary>
        /// <param name="model">Modelo de Pré-Registro a ser validado (ex: Nome, Email, Senha).</param>
        /// <returns>Redireciona o usuário para página de confirmação de código se as informações forem válidas.</returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Password = string.Empty;
                TempData[RegisterModelKey] = JsonSerializer.Serialize(model);
                TempData[ErrorMessageKey] = InvalidDataMessage;
                return RedirectToAction("Register", "Login");
            }

            var result = await _registerService.GenerateCodeAndSentEmailAsync(model);

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
        /// Rota get protegida com Token de permissão de acesso.
        /// </summary>
        [HttpGet]
        public IActionResult ConfirmCode()
        {
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
                TempData[ErrorMessageKey] = SessionExpiredMessage;
                return RedirectToAction("Register", "Login");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                TempData.Keep(RegisterModelKey);
                TempData[AllowAccessConfirmationKey] = true;
                TempData[ErrorMessageKey] = "Por favor, digite o código de confirmação.";
                return RedirectToAction(nameof(ConfirmCode));
            }

            var result = await _confirmationCodeService.VerifyCodeAndCreateUserAccountAsync(emailString, code);

            if (!result.IsSuccess)
            {
                TempData.Keep(RegisterModelKey);
                TempData[AllowAccessConfirmationKey] = true;
                TempData[ErrorMessageKey] = result.ErrorMessage;
                return RedirectToAction(nameof(ConfirmCode));
            }

            TempData["SuccessMessage"] = SuccessMessage;
            return RedirectToAction("Index", "Home");
        }
    }
}
