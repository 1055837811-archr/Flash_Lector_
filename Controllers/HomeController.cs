using System.Security.Claims;
using Flash_Lector.Data;
using Flash_Lector.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Controllers;

public class HomeController(ContextBaseDatos bd) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.IdsFavoritos = await bd.IdsFavoritosAsync(User);

        var texto = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(texto, out var usuarioId))
        {
            ViewBag.Continuar = await bd.ProgresosLectura
                .Include(x => x.Obra).ThenInclude(o => o.Categoria)
                .Where(x => x.UsuarioId == usuarioId && x.Obra.Estado == EstadoPublicacion.Publicada)
                .OrderByDescending(x => x.FechaActualizacion).FirstOrDefaultAsync();
            ViewBag.Nombres = await bd.Usuarios.Where(u => u.Id == usuarioId).Select(u => u.Nombres).FirstOrDefaultAsync();
        }

        var obras = await bd.Obras.Include(x => x.Categoria)
            .Where(x => x.Estado == EstadoPublicacion.Publicada)
            .OrderByDescending(x => x.FechaPublicacion).Take(12).ToListAsync();
        return View(obras);
    }

    public IActionResult Error() => View();
}
