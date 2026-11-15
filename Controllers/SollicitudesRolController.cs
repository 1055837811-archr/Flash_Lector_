using Flash_Lector.Data;
using System.Security.Claims;
using Flash_Lector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Controllers;

[Authorize]
public class SolicitudesRolController(ContextBaseDatos bd) : Controller
{
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (!await bd.SolicitudesRol.AnyAsync(x => x.UsuarioId == usuarioId && x.Estado == EstadoSolicitud.Pendiente))
        {
            bd.SolicitudesRol.Add(new SolicitudRol { UsuarioId = usuarioId, RolSolicitado = "Creador" });
            await bd.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Obras");
    }
}