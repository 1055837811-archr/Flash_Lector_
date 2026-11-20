using System.Security.Claims;
using Flash_Lector.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Controllers;

public class DevController(ContextBaseDatos bd) : Controller
{
    // Cambio de rol para pruebas: solo lo puede usar un administrador
    // (o quien ya entró por este mismo atajo, para poder volver).
    public async Task<IActionResult> Como(string rol)
    {
        var autorizado = User.IsInRole("Administrador") || User.HasClaim("PuedeCambiarRol", "true");
        if (!autorizado) return NotFound();

        if (rol == "acceso")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("InicioSesion", "Cuenta");
        }

        var correo = rol switch
        {
            "lector" => "lector@flashlector.local",
            "creador" => "autor@flashlector.local",
            "admin" => "admin@flashlector.local",
            _ => null
        };
        if (correo is null) return NotFound();

        var usuario = await bd.Usuarios.FirstOrDefaultAsync(x => x.Correo == correo);
        if (usuario is null) return NotFound();

        var datos = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, $"{usuario.Nombres} {usuario.Apellidos}"),
            new(ClaimTypes.Email, usuario.Correo),
            new(ClaimTypes.Role, usuario.Rol),
            new("PuedeCambiarRol", "true")
        };
        var identidad = new ClaimsIdentity(datos, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));

        return RedirectToAction("Index", "Home");
    }
}