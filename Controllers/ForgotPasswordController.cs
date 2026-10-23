using Microsoft.AspNetCore.Mvc;
using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;
using Portifolio.Services.PasswordRecovery;

namespace Portifolio.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private const string AllowAccessConfirmationKey = "AllowAccessConfirmation";
        private const string ErrorMessageKey = "ErrorMessage";
        private const string EmailModelKey = "EmailModel";
        private const string SessionExpiredMessage = "Sua sessão expirou. Por favor, reinicie o processo de recuperação de senha.";

        private readonly IRecoverPassword _recoverPasswordService;
        private readonly IConfirmCodePasswordRecoverService _confirmationCodeService;

        public ForgotPasswordController(IRecoverPassword recoverPassword, IConfirmCodePasswordRecoverService confirmationCodeService)
        {
            _recoverPasswordService = recoverPassword;
            _confirmationCodeService = confirmationCodeService;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewBag.ErrorMessage = TempData[ErrorMessageKey]?.ToString();
            return View();
        }

        /// <summary>
        /// Rota post que recebe o e-email de usuário para recuprar senhar.
        /// </summary>
        /// <param name="model">Modelo de e-mail informado pelo usuário.</param>
        /// <returns>Redireciona para tela de confirmação do código.</returns>
        [HttpPost]
        public async Task<IActionResult> PasswordRecoverEmail(EmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(ForgotPassword), model);
            }

            PreRegisterResult result = await _recoverPasswordService.GenerateCodeAndSendEmailAsync(model);

            if (!result.IsSuccess)
            {
                TempData[ErrorMessageKey] = result.ErrorMessage;
                return RedirectToAction(nameof(ForgotPassword));
            }

            TempData[EmailModelKey] = model.Email.ToLower();
            TempData[AllowAccessConfirmationKey] = true;
            return RedirectToAction(nameof(ConfirmCodePasswordRecover));
        }

        /// <summary>
        /// Rota get protegida que renderiza a tela para o usuário digitar código de confirmação.
        /// </summary>
        [HttpGet]
        public IActionResult ConfirmCodePasswordRecover()
        {
            var email = TempData.Peek(EmailModelKey)?.ToString();
            var allowAcess = TempData.Peek(AllowAccessConfirmationKey)?.ToString();

            if (email is null || allowAcess is null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = TempData[ErrorMessageKey]?.ToString();
            ViewBag.Email = email;
            return View();
        }

        /// <summary>
        /// Rota post que faz a verificação do código digitado pelo usuário.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ValidateCodePasswordRecover(string code)
        {
            string email = TempData.Peek(EmailModelKey)?.ToString() ?? string.Empty;
            var allowAccess = TempData.Peek(AllowAccessConfirmationKey)?.ToString();

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData[ErrorMessageKey] = SessionExpiredMessage;
                return RedirectToAction(nameof(ForgotPassword));
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                TempData[AllowAccessConfirmationKey] = true;
                TempData[ErrorMessageKey] = "Por favor, digite o código de confirmação.";
                return RedirectToAction(nameof(ConfirmCodePasswordRecover));
            }

            PreRegisterResult result = await _confirmationCodeService.ValidateCodePasswordRecoverAsync(email, code);

            if (!result.IsSuccess)
            {
                TempData[AllowAccessConfirmationKey] = true;
                TempData[ErrorMessageKey] = result.ErrorMessage;
                return RedirectToAction(nameof(ConfirmCodePasswordRecover));
            }

            TempData[EmailModelKey] = email;
            TempData[AllowAccessConfirmationKey] = allowAccess;
            return RedirectToAction("ResetPassword", "ResetPassword");
        }
    }
}
