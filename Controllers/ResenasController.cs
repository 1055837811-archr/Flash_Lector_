using System.Security.Claims;
using Flash_Lector.Data;
using Flash_Lector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Controllers;

[Authorize]
public class ResenasController(ContextBaseDatos bd) : Controller
{
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(int obraId, int calificacion, string comentario)
    {
        var obra = await bd.Obras.FirstOrDefaultAsync(x => x.Id == obraId);
        if (obra is null) return NotFound();

        if (calificacion is < 1 or > 5 || string.IsNullOrWhiteSpace(comentario) || comentario.Length > 1000)
        {
            TempData["ErrorResena"] = "La calificación y el comentario no son válidos.";
            return RedirectToAction("Detalle", "Catalogo", new { id = obraId });
        }

        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        bd.Resenas.Add(new Resena { ObraId = obraId, UsuarioId = usuarioId, Calificacion = calificacion, Comentario = comentario.Trim() });
        await bd.SaveChangesAsync();

        return RedirectToAction("Detalle", "Catalogo", new { id = obraId });
    }
}