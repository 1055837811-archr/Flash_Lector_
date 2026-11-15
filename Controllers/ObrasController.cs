using System.Security.Claims;
using Flash_Lector.Data;
using Flash_Lector.Models;
using Flash_Lector.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Controllers;

[Authorize]
public class ObrasController(ContextBaseDatos bd, IWebHostEnvironment entorno) : Controller
{
    private const long TamanoMaximoBytes = 25 * 1024 * 1024;

    // La cookie guarda el id como texto; con [Authorize] siempre existe
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var usuarioId = UsuarioId;
        return View(await bd.Obras.Include(x => x.Categoria).Where(x => x.CreadorId == usuarioId).OrderByDescending(x => x.Id).ToListAsync());
    }

    [HttpGet, Authorize(Roles = "Creador,Administrador")]
    public async Task<IActionResult> Crear()
    {
        await CargarCategorias();
        return View(new ObraFormularioViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Creador,Administrador")]
    public async Task<IActionResult> Crear(ObraFormularioViewModel modelo)
    {
        if (modelo.Archivo is null)
            ModelState.AddModelError(nameof(modelo.Archivo), "Seleccione el archivo de la obra.");
        else if (ErrorArchivo(modelo.Archivo, modelo.Formato) is { } errorArchivo)
            ModelState.AddModelError(nameof(modelo.Archivo), errorArchivo);

        if (!ModelState.IsValid)
        {
            await CargarCategorias();
            return View(modelo);
        }

        var rutaArchivo = await GuardarArchivoAsync(modelo.Archivo!);
        bd.Obras.Add(new Obra
        {
            Titulo = modelo.Titulo.Trim(),
            NombreAutor = modelo.NombreAutor.Trim(),
            Sinopsis = modelo.Sinopsis.Trim(),
            Paginas = modelo.Paginas,
            Formato = modelo.Formato,
            CategoriaId = modelo.CategoriaId,
            CreadorId = UsuarioId,
            RutaArchivo = rutaArchivo,
            Estado = EstadoPublicacion.EnRevision
        });
        await bd.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, Authorize(Roles = "Creador,Administrador")]
    public async Task<IActionResult> Editar(int id)
    {
        var obra = await ObraPropia(id);
        if (obra is null) return NotFound();

        await CargarCategorias();
        return View(new ObraFormularioViewModel
        {
            Id = obra.Id,
            Titulo = obra.Titulo,
            NombreAutor = obra.NombreAutor,
            Sinopsis = obra.Sinopsis,
            Paginas = obra.Paginas,
            Formato = obra.Formato,
            CategoriaId = obra.CategoriaId
        });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Creador,Administrador")]
    public async Task<IActionResult> Editar(ObraFormularioViewModel modelo)
    {
        var obra = await ObraPropia(modelo.Id);
        if (obra is null) return NotFound();

        if (modelo.Archivo is not null && ErrorArchivo(modelo.Archivo, modelo.Formato) is { } errorArchivo)
            ModelState.AddModelError(nameof(modelo.Archivo), errorArchivo);

        if (!ModelState.IsValid)
        {
            await CargarCategorias();
            return View(modelo);
        }

        if (modelo.Archivo is not null)
        {
            BorrarArchivo(obra.RutaArchivo);
            obra.RutaArchivo = await GuardarArchivoAsync(modelo.Archivo);
        }

        obra.Titulo = modelo.Titulo.Trim();
        obra.NombreAutor = modelo.NombreAutor.Trim();
        obra.Sinopsis = modelo.Sinopsis.Trim();
        obra.Paginas = modelo.Paginas;
        obra.Formato = modelo.Formato;
        obra.CategoriaId = modelo.CategoriaId;
        obra.Estado = EstadoPublicacion.EnRevision;

        await bd.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Creador,Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var obra = await ObraPropia(id);
        if (obra is null) return NotFound();

        BorrarArchivo(obra.RutaArchivo);
        bd.Obras.Remove(obra);
        await bd.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Revisa que el archivo no esté vacío, que pese menos de 25 MB y que su extensión coincida con el formato
    private static string? ErrorArchivo(IFormFile archivo, FormatoObra formato)
    {
        var esperada = "." + formato.ToString().ToLowerInvariant();
        if (archivo.Length == 0) return "El archivo está vacío.";
        if (archivo.Length > TamanoMaximoBytes) return "El archivo supera los 25 MB.";
        if (!string.Equals(Path.GetExtension(archivo.FileName), esperada, StringComparison.OrdinalIgnoreCase))
            return $"El archivo debe ser {esperada} para el formato {formato}.";
        return null;
    }

    private async Task<string> GuardarArchivoAsync(IFormFile archivo)
    {
        var carpeta = Path.Combine(entorno.WebRootPath, "uploads");
        Directory.CreateDirectory(carpeta);
        var nombre = Guid.NewGuid().ToString("N") + Path.GetExtension(archivo.FileName).ToLowerInvariant();
        await using var flujo = System.IO.File.Create(Path.Combine(carpeta, nombre));
        await archivo.CopyToAsync(flujo);
        return "/uploads/" + nombre;
    }

    private void BorrarArchivo(string? rutaArchivo)
    {
        if (rutaArchivo is null || !rutaArchivo.StartsWith("/uploads/")) return;
        var completa = Path.Combine(entorno.WebRootPath, "uploads", Path.GetFileName(rutaArchivo));
        if (System.IO.File.Exists(completa)) System.IO.File.Delete(completa);
    }

    private Task<Obra?> ObraPropia(int id)
    {
        var usuarioId = UsuarioId;
        return bd.Obras.FirstOrDefaultAsync(x => x.Id == id && x.CreadorId == usuarioId);
    }

    private async Task CargarCategorias() =>
        ViewBag.Categorias = new SelectList(await bd.Categorias.OrderBy(x => x.Nombre).ToListAsync(), "Id", "Nombre");
}