using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portifolio.Models.ViewModels;
using Portifolio.Services.Authentication;

namespace Portifolio.Controllers
{
    /// <summary>
    /// Controla o fluxo de login e sessão do usuário.
    /// </summary>
    [Authorize]
    public class LoginController : Controller
    {
        private readonly ILoginService _authService;

        public LoginController(ILoginService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Rota GET padrão que renderiza o formulário de login.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Panel));
            }
            return View();
        }

        /// <summary>
        /// Rota post para autenticação do usuário no login.
        /// </summary>
        /// <param name="model">Credenciais informadas pelo usuário (email e senha).</param>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromForm] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Password = string.Empty;
                TempData["LoginModel"] = JsonSerializer.Serialize(model);
                TempData["ErrorMessage"] = "Dados informados em formato inválido. Por favor, verifique as informações.";

                return RedirectToAction(nameof(Index));
            }

            var user = await _authService.AuthenticateAsync(model);

            if (user == null)
            {
                model.Password = string.Empty;
                TempData["LoginModel"] = JsonSerializer.Serialize(model);
                TempData["ErrorMessage"] = "Usuário e/ou senha incorretos.";

                return RedirectToAction("Index", "Home");
            }

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("Id", user.Id.ToString()),
                new System.Security.Claims.Claim("Name", user.Name),
                new System.Security.Claims.Claim("Email", user.Email)
            };

            var claimsIdentity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new System.Security.Claims.ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
            });

            return RedirectToAction(nameof(Panel));
        }

        /// <summary>
        /// Rota GET protegida que exibe o painel administrativo após o login bem-sucedido.
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult Panel()
        {
            return View();
        }

        /// <summary>
        /// Rota GET que carrega a página para criar nova conta.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Realiza o encerramento da sessão atual do usuário limpando os cookies.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}
