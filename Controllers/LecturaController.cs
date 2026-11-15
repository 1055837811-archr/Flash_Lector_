using System.Security.Claims;
using Flash_Lector.Data;
using Flash_Lector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Controllers;

[Authorize]
public class LecturaController(ContextBaseDatos bd) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Leer(int id)
    {
        var obra = await bd.Obras.Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == id && x.Estado == EstadoPublicacion.Publicada);
        if (obra is null || string.IsNullOrEmpty(obra.RutaArchivo)) return NotFound();

        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var progreso = await bd.ProgresosLectura.FindAsync(usuarioId, obra.Id);
        ViewBag.Pagina = progreso?.PaginaActual ?? 1;
        return View(obra);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(int obraId, int paginaActual)
    {
        var obra = await bd.Obras.FirstOrDefaultAsync(x => x.Id == obraId);
        if (obra is null) return NotFound();

        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var progreso = await bd.ProgresosLectura.FindAsync(usuarioId, obraId);

        if (progreso is null)
            bd.ProgresosLectura.Add(new ProgresoLectura { UsuarioId = usuarioId, ObraId = obraId, PaginaActual = Math.Clamp(paginaActual, 1, obra.Paginas) });
        else
        {
            progreso.PaginaActual = Math.Clamp(paginaActual, 1, obra.Paginas);
            progreso.FechaActualizacion = DateTime.UtcNow;
        }

        await bd.SaveChangesAsync();
        return RedirectToAction(nameof(Leer), new { id = obra.Id });
    }
}
