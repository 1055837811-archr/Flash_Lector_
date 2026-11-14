using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Data;

public static class ExtensionesFavorito
{
    // Ids de las obras favoritas del usuario actual (vacío si no ha iniciado sesión).
    public static async Task<HashSet<int>> IdsFavoritosAsync(this ContextBaseDatos bd, ClaimsPrincipal usuario)
    {
        var texto = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(texto, out var usuarioId)) return [];
        return [.. await bd.Favoritos.Where(x => x.UsuarioId == usuarioId).Select(x => x.ObraId).ToListAsync()];
    }
}