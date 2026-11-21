using System.Security.Claims;
using Flash_Lector.Data;
using Flash_Lector.Models;
using Flash_Lector.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Controllers;

// Inyección de dependencias: el contexto de la base de datos y el encriptador de contraseñas
public class CuentaController(ContextBaseDatos bd, IPasswordHasher<Usuario> encriptador) : Controller
{
    [AllowAnonymous] // permite entrar sin haber iniciado sesión
    [HttpGet]
    public IActionResult InicioSesion(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl; // guarda la página a la que quería entrar
        return View();
    }

    [AllowAnonymous]
    [HttpPost, ValidateAntiForgeryToken] // ValidateAntiForgeryToken protege el formulario contra ataques CSRF
    public async Task<IActionResult> InicioSesion(InicioSesionViewModel modelo, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(modelo);

        // Se normaliza el correo: sin espacios y en minúsculas
        var correo = modelo.Correo.Trim().ToLowerInvariant();

        var usuario = await bd.Usuarios.SingleOrDefaultAsync(x => x.Correo == correo);

        // Si el usuario no existe o la contraseña no coincide, las credenciales son incorrectas
        if (usuario is null ||
            encriptador.VerifyHashedPassword(usuario, usuario.ContrasenaHash, modelo.Contrasena) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View(modelo);
        }

        await CrearCookieAsync(usuario, modelo.Recordarme);

        // Si venía de una página protegida, vuelve a ella (solo direcciones del mismo sitio)
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Registro() => View();

    [AllowAnonymous]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var correo = modelo.Correo.Trim().ToLowerInvariant();

        if (await bd.Usuarios.AnyAsync(x => x.Correo == correo))
        {
            ModelState.AddModelError("", "Ya existe una cuenta con ese correo.");
            return View(modelo);
        }

        var usuario = new Usuario
        {
            Nombres = modelo.Nombres.Trim(),
            Apellidos = modelo.Apellidos.Trim(),
            Correo = correo,
            Rol = "Lector"
        };
        usuario.ContrasenaHash = encriptador.HashPassword(usuario, modelo.Contrasena);

        bd.Usuarios.Add(usuario);
        await bd.SaveChangesAsync();

        await CrearCookieAsync(usuario, false);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CerrarSesion()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccesoDenegado() => View();

    // Los claims guardan en la cookie la información importante del usuario: id, nombre, correo y rol
    private async Task CrearCookieAsync(Usuario usuario, bool recordarme)
    {
        var datos = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, $"{usuario.Nombres} {usuario.Apellidos}"),
            new(ClaimTypes.Email, usuario.Correo),
            new(ClaimTypes.Role, usuario.Rol)
        };

        var identidad = new ClaimsIdentity(datos, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad),
            new AuthenticationProperties
            {
                IsPersistent = recordarme, // la sesión sigue abierta aunque se cierre el navegador
                AllowRefresh = true        // la sesión se renueva sola mientras el usuario esté activo
            });
    }
}