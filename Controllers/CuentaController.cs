using System.Security.Claims;
using Flash_Lector.Data;
using Flash_Lector.Models;
using Flash_Lector.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Controllers;

public class CuentaController(ContextBaseDatos bd, IPasswordHasher<Usuario> encriptador) : Controller
{
    [HttpGet]
    public IActionResult InicioSesion() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> InicioSesion(InicioSesionViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var usuario = await bd.Usuarios.FirstOrDefaultAsync(x => x.Correo == modelo.Correo);

        if (usuario != null &&
            encriptador.VerifyHashedPassword(usuario, usuario.ContrasenaHash, modelo.Contrasena) != PasswordVerificationResult.Failed)
        {
            await CrearCookieAsync(usuario, modelo.Recordarme);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Correo o contraseña incorrectos.");
        return View(modelo);
    }

    [HttpGet]
    public IActionResult Registro() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        if (await bd.Usuarios.AnyAsync(x => x.Correo == modelo.Correo))
        {
            ModelState.AddModelError("", "Ya existe una cuenta con ese correo.");
            return View(modelo);
        }

        var usuario = new Usuario
        {
            Nombres = modelo.Nombres,
            Apellidos = modelo.Apellidos,
            Correo = modelo.Correo,
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

    [HttpGet]
    public IActionResult AccesoDenegado() => View();

    // Crea la cookie de sesión con el id, el nombre, el correo y el rol del usuario.
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
        var propiedades = new AuthenticationProperties { IsPersistent = recordarme };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad),
            propiedades);
    }
}