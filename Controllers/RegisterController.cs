using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Register;

namespace Portifolio.Controllers
{   
    [Authorize]
    public class RegisterController : Controller
    {
        private readonly IRegisterService _registerService;
        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            // Executa a validação e o pré-registro (que por sua vez dispara o e-mail)
            if (!ModelState.IsValid)
            {
                TempData["RegisterModel"] = JsonSerializer.Serialize(model);
                
                TempData["ErrorMessage"] = "Os dados informados estão em um formato inválido.";

                return RedirectToAction("Register", "Login");
            }

            var result = await _registerService.PreRegisterAsync(model);

            if (!result.IsSuccess)
            {
                TempData["RegisterModel"] = JsonSerializer.Serialize(model);

                TempData["ErrorMessage"] = result.ErrorMessage;

                return RedirectToAction("Register", "Login");
            }

            // RETORNO TEMPORÁRIO PARA TESTE:
            return Content($"Sucesso! O pré-registro foi feito e o código de 6 dígitos foi enviado para: {model.Email}. Verifique sua caixa de entrada (e a pasta de Spam/Lixo Eletrônico).");
        }
    }
}
