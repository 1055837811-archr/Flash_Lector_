using System.Security.Claims;
using Flash_Lector.Data;
using Flash_Lector.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Controllers;

public class CatalogoController(ContextBaseDatos bd) : Controller
{
    public async Task<IActionResult> Index(string? q, int? categoria)
    {
        var consulta = bd.Obras
            .Include(x => x.Categoria)
            .Where(x => x.Estado == EstadoPublicacion.Publicada);

        if (!string.IsNullOrWhiteSpace(q))
            consulta = consulta.Where(x => x.Titulo.Contains(q) || x.NombreAutor.Contains(q));

        if (categoria.HasValue)
            consulta = consulta.Where(x => x.CategoriaId == categoria);

        ViewBag.Categorias = await bd.Categorias.ToListAsync();
        ViewBag.IdsFavoritos = await bd.IdsFavoritosAsync(User);

        return View(await consulta.ToListAsync());
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var obra = await bd.Obras
            .Include(x => x.Categoria)
            .Include(x => x.Resenas.Where(r => r.EsVisible))
                .ThenInclude(r => r.Usuario)
            .FirstOrDefaultAsync(x => x.Id == id && x.Estado == EstadoPublicacion.Publicada);

        if (obra is null)
            return NotFound();

        var texto = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ViewBag.EsFavorito = int.TryParse(texto, out var usuarioId) &&
            await bd.Favoritos.AnyAsync(x => x.UsuarioId == usuarioId && x.ObraId == obra.Id);

        return View(obra);
    }
}