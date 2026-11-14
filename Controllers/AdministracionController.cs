using Flash_Lector.Data;
using Flash_Lector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Flash_Lector.Controllers;

[Authorize(Roles = "Administrador")]
public class AdministracionController(ContextBaseDatos bd) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Usuarios = await bd.Usuarios.CountAsync();
        ViewBag.ObrasPendientes = await bd.Obras.CountAsync(x => x.Estado == EstadoPublicacion.EnRevision);
        ViewBag.RolesPendientes = await bd.SolicitudesRol.CountAsync(x => x.Estado == EstadoSolicitud.Pendiente);
        ViewBag.Solicitudes = await bd.SolicitudesRol.Include(x => x.Usuario).Where(x => x.Estado == EstadoSolicitud.Pendiente).OrderBy(x => x.Id).ToListAsync();
        return View(await bd.Obras.Include(x => x.Categoria).OrderByDescending(x => x.Id).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, EstadoPublicacion estado)
    {
        var obra = await bd.Obras.FindAsync(id);
        if (obra is null) return NotFound();
        obra.Estado = estado;
        if (estado == EstadoPublicacion.Publicada) obra.FechaPublicacion ??= DateTime.UtcNow;
        await bd.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResolverRol(int id, bool aprobar)
    {
        var solicitud = await bd.SolicitudesRol.Include(x => x.Usuario).FirstOrDefaultAsync(x => x.Id == id && x.Estado == EstadoSolicitud.Pendiente);
        if (solicitud is null) return NotFound();
        if (aprobar)
            solicitud.Usuario.Rol = solicitud.RolSolicitado;
        solicitud.Estado = aprobar ? EstadoSolicitud.Aprobada : EstadoSolicitud.Rechazada;
        await bd.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}