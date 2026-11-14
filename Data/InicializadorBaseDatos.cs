using Flash_Lector.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Flash_Lector.Data;

public static class InicializadorBaseDatos
{
    public static async Task InicializarAsync(IServiceProvider servicios)
    {
        var bd = servicios.GetRequiredService<ContextBaseDatos>();
        var encriptador = servicios.GetRequiredService<IPasswordHasher<Usuario>>();

        // Si existen migraciones se aplican; si todavía no se generaron, se crea el esquema directamente
        // para que el proyecto abra y corra desde el primer intento.
        if (bd.Database.GetMigrations().Any())
            await bd.Database.MigrateAsync();
        else
            await bd.Database.EnsureCreatedAsync();

        // Usuario administrador de desarrollo (cámbielo en producción).
        await CrearUsuarioAsync(bd, encriptador, "admin@flashlector.local", "Admin12345!", "Admin", "Flash Lector", "Administrador");

        // Usuarios de demostración para probar cada rol (solo se crean en Development).
        if (servicios.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            await CrearUsuarioAsync(bd, encriptador, "lector@flashlector.local", "Lector12345!", "Lector", "Demo", "Lector");
            await CrearUsuarioAsync(bd, encriptador, "autor@flashlector.local", "Autor12345!", "Autor", "Demo", "Creador");
        }

        if (!await bd.Categorias.AnyAsync())
            await CrearCatalogoAsync(bd);
    }

    private static async Task CrearUsuarioAsync(ContextBaseDatos bd, IPasswordHasher<Usuario> encriptador, string correo, string contrasena, string nombres, string apellidos, string rol)
    {
        if (await bd.Usuarios.AnyAsync(x => x.Correo == correo)) return;

        var usuario = new Usuario { Correo = correo, Nombres = nombres, Apellidos = apellidos, Rol = rol };
        usuario.ContrasenaHash = encriptador.HashPassword(usuario, contrasena);

        bd.Usuarios.Add(usuario);
        await bd.SaveChangesAsync();
    }

    private static async Task CrearCatalogoAsync(ContextBaseDatos bd)
    {
        var categorias = new[]
        {
            new Categoria { Nombre = "Académico" },
            new Categoria { Nombre = "Literatura" },
            new Categoria { Nombre = "Cómics" },
            new Categoria { Nombre = "Mangas" }
        };
        bd.Categorias.AddRange(categorias);
        await bd.SaveChangesAsync();

        // Las obras de muestra usan los PDF de la carpeta wwwroot/samples.
        // El formato se deja variado (EPUB, CBZ...) para que el catálogo se vea diverso.
        bd.Obras.AddRange(
            new Obra { Titulo = "Algoritmos cotidianos", NombreAutor = "Lucía Velasco", Sinopsis = "Ideas de sistemas aplicadas a decisiones de todos los días.", Formato = FormatoObra.EPUB, Estado = EstadoPublicacion.Publicada, Paginas = 6, RutaArchivo = "/samples/algoritmos-cotidianos.pdf", CategoriaId = categorias[0].Id, FechaPublicacion = DateTime.UtcNow },
            new Obra { Titulo = "La ciudad de los ecos", NombreAutor = "Tomás Niebla", Sinopsis = "Una ciudad conserva las voces de quienes la recorren.", Formato = FormatoObra.PDF, Estado = EstadoPublicacion.Publicada, Paginas = 6, RutaArchivo = "/samples/la-ciudad-de-los-ecos.pdf", CategoriaId = categorias[1].Id, FechaPublicacion = DateTime.UtcNow },
            new Obra { Titulo = "Órbita cero", NombreAutor = "Mara Céspedes", Sinopsis = "Una tripulación estudiantil descifra una señal fuera de ruta.", Formato = FormatoObra.CBZ, Estado = EstadoPublicacion.Publicada, Paginas = 6, RutaArchivo = "/samples/orbita-cero.pdf", CategoriaId = categorias[2].Id, FechaPublicacion = DateTime.UtcNow },
            new Obra { Titulo = "Cálculo para la vida", NombreAutor = "Rafael Montoya", Sinopsis = "Límites, derivadas e integrales explicadas con situaciones reales.", Formato = FormatoObra.PDF, Estado = EstadoPublicacion.Publicada, Paginas = 6, RutaArchivo = "/samples/calculo-para-la-vida.pdf", CategoriaId = categorias[0].Id, FechaPublicacion = DateTime.UtcNow.AddDays(-3) });
        await bd.SaveChangesAsync();
    }
}