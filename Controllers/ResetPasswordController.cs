using Microsoft.AspNetCore.Mvc;
using Portifolio.Models.Result;
using Portifolio.Models.ViewModels;
using Portifolio.Services.PasswordReset;

namespace Portifolio.Controllers
{
    /// <summary>
    /// Controla o fluxo de recuperação de senha.
    /// </summary>
    public class ResetPasswordController : Controller
    {
        private const string AllowAccessConfirmationKey = "AllowAccessConfirmation";
        private const string EmailModelKey = "EmailModel";
        private const string ErrorMessageKey = "ErrorMessage";
        private const string SessionExpiredMessage = "Sua sessão expirou. Por favor, reinicie o processo de recuperação de senha.";
        private const string SuccessMessage = "Sua senha foi salva com sucesso.";

        private readonly IPasswordResetService _passwordResetService;

        public ResetPasswordController(IPasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        /// <summary>
        /// Renderiza a tela para o usuário digitar a nova senha. 
        /// </summary>
        [HttpGet]
        public IActionResult ResetPassword()
        {
            string email = TempData.Peek(EmailModelKey)?.ToString() ?? string.Empty;
            string tokenAccess = TempData.Peek(AllowAccessConfirmationKey)?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(tokenAccess))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = TempData[ErrorMessageKey]?.ToString();
            return View();
        }

        /// <summary>
        /// Rota protegida contra ataque direto via Postman. Faz a validação da nova senha.
        /// </summary>
        /// <param name="model">Modelo de recuperação de senha (ex: E-mail, Nova Senha)</param>
        /// <returns>Redireciona o usuário para a tela de login em caso de sucesso. Senão, exibe mensagem de erro.</returns>
        [HttpPost]
        public async Task<IActionResult> SaveNewPassword([FromForm] ResetPasswordViewModel model)
        {
            string sessionEmail = TempData.Peek(EmailModelKey)?.ToString() ?? string.Empty;
            string tokenAccess = TempData.Peek(AllowAccessConfirmationKey)?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(sessionEmail) || string.IsNullOrEmpty(tokenAccess))
            {
                TempData[ErrorMessageKey] = SessionExpiredMessage;
                return RedirectToAction("Index", "Home");
            }

            model.Email = sessionEmail;
            ModelState.Remove(nameof(model.Email));

            if (!ModelState.IsValid)
            {
                return View(nameof(ResetPassword), model);
            }

            PreRegisterResult result = await _passwordResetService.SaveNewPasswordAsync(model);

            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessage = result.ErrorMessage;
                return View(nameof(ResetPassword), model);
            }

            TempData.Remove(EmailModelKey);
            TempData.Remove(AllowAccessConfirmationKey);
            TempData["SuccessMessage"] = SuccessMessage;
            return RedirectToAction("Index", "Home");
        }
    }
}
