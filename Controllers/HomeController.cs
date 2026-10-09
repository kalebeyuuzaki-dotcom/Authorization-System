using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Portifolio.Models.ViewModels;

namespace Portifolio.Controllers;

/// <summary>
/// Páginas iniciais do site.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Página inicial com form para realizar login.
    /// </summary>
    /// <returns>Renderiza a página de login.</returns>
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
