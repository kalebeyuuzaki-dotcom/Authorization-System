using Microsoft.AspNetCore.Mvc;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Implements;
using Portifolio.Services.Interfaces;

namespace Portifolio.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;
        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromForm] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml", model);
            }

            bool isAuhtenticate = await _authService.AuthAsync(model);

            if (isAuhtenticate == false)
            {
                ViewData["MessageFail"] = "Usuário e/ou senha incorretos.";

                return View("~/Views/Home/Index.cshtml", model);
            }

            return RedirectToAction(nameof(Panel));
        }

        [HttpGet]
        public IActionResult Panel()
        {
            return View();
        }
    }
}