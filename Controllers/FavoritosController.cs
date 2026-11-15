using System.Security.Claims;
using Flash_Lector.Data;
using Flash_Lector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Controllers;

[Authorize]
public class FavoritosController(ContextBaseDatos bd) : Controller
{
    public async Task<IActionResult> Index()
    {
        // La cookie guarda el id como texto; con [Authorize] siempre existe
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        ViewBag.IdsFavoritos = await bd.IdsFavoritosAsync(User);

        return View(await bd.Favoritos
            .Where(x => x.UsuarioId == usuarioId)
            .Include(x => x.Obra).ThenInclude(o => o.Categoria)
            .ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Alternar(int obraId, string? urlRetorno)
    {
        var obra = await bd.Obras.FirstOrDefaultAsync(x => x.Id == obraId);
        if (obra is null)
            return NotFound();

        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var favorito = await bd.Favoritos.FindAsync(usuarioId, obraId);

        if (favorito is null)
            bd.Favoritos.Add(new Favorito { UsuarioId = usuarioId, ObraId = obraId });
        else
            bd.Favoritos.Remove(favorito);

        await bd.SaveChangesAsync();

        if (!string.IsNullOrEmpty(urlRetorno) && Url.IsLocalUrl(urlRetorno))
            return LocalRedirect(urlRetorno);

        return RedirectToAction("Detalle", "Catalogo", new { id = obraId });
    }
}
