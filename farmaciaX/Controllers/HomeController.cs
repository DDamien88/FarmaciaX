using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using farmaciaX.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace farmaciaX.Controllers;

public class HomeController : Controller
{
    //private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration config;

    public HomeController(ILogger<HomeController> logger, IConfiguration config)
    {
        this.config = config;
        logger.LogInformation("HomeController creado");
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        var modelo = new LoginView();
        return View(modelo);
    }

    public IActionResult Privacy()
    {
        return View();
    }



    [Authorize(Policy = "Administrador")]
    public ActionResult Admin()
    {
        return View();
    }

    public ActionResult Restringido()
    {
        return View();
    }

    // [Authorize]
    // public async Task<ActionResult> CambiarClaim()
    // {
    //     var identity = (ClaimsIdentity)User.Identity;
    //     identity.RemoveClaim(identity.FindFirst("FullName"));
    //     identity.AddClaim(new Claim("FullName", "Cosme Fulanito"));
    //     await HttpContext.SignInAsync(
    //         CookieAuthenticationDefaults.AuthenticationScheme,
    //         new ClaimsPrincipal(identity));
    //     return Redirect(nameof(Seguro));
    // }

    public IActionResult Fecha(int anio, int mes, int dia)
    {
        DateTime dt = new DateTime(anio, mes, dia);
        ViewBag.Fecha = dt;
        return View();
    }

    public IActionResult Ruta(string valor)
    {
        DateTime posibleFecha;
        if (DateTime.TryParse(valor, out posibleFecha))
        {
            ViewBag.Valor = "Escribiste una fecha: " + posibleFecha.ToShortDateString();
        }
        else
        {
            ViewBag.Valor = valor;
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    // Get Home/Seguro.cshtml
    [Authorize]
public IActionResult Seguro()
{
    var claims = User.Claims.ToList();
    return View(claims);
}

}
