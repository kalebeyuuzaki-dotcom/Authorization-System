using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portifolio.Models.ViewModels;

namespace Portifolio.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (TempData["LoginModel"] is string modelJson)
        {
            var model = JsonSerializer.Deserialize<LoginViewModel>(modelJson);

            return View(model);
        }

        return View();
    }
    [HttpGet]
    [AcceptVerbs("GET", "POST")]
    [DisableRateLimiting]
    public IActionResult TooManyRequests()
    {
        ViewData["ErrorMessage"] = "Muitas tentativas detectadas. Por medidas de segurança, este IP foi bloqueado temporariamente.";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
