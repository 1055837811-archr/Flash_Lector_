using Flash_Lector.Models;
using Microsoft.EntityFrameworkCore;

namespace Flash_Lector.Data;

public class ContextBaseDatos(DbContextOptions<ContextBaseDatos> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Obra> Obras => Set<Obra>();
    public DbSet<Favorito> Favoritos => Set<Favorito>();
    public DbSet<ProgresoLectura> ProgresosLectura => Set<ProgresoLectura>();
    public DbSet<Resena> Resenas => Set<Resena>();
    public DbSet<SolicitudRol> SolicitudesRol => Set<SolicitudRol>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // No se permiten dos usuarios con el mismo correo
        modelBuilder.Entity<Usuario>().HasIndex(x => x.Correo).IsUnique();

        // Llaves primarias compuestas (usuario + obra)
        modelBuilder.Entity<Favorito>().HasKey(x => new { x.UsuarioId, x.ObraId });
        modelBuilder.Entity<ProgresoLectura>().HasKey(x => new { x.UsuarioId, x.ObraId });

        // Si se borra el usuario creador, la obra se conserva sin creador
        modelBuilder.Entity<Obra>().HasOne(x => x.Creador).WithMany(x => x.Obras)
            .HasForeignKey(x => x.CreadorId).OnDelete(DeleteBehavior.SetNull);

        // SQL Server no permite varias rutas de borrado en cascada
        // (Usuario -> Obra -> Favorito y Usuario -> Favorito).
        // ClientCascade hace el borrado desde el código y evita crear la segunda cascada en la base de datos.
        modelBuilder.Entity<Favorito>().HasOne(x => x.Usuario).WithMany(x => x.Favoritos)
            .HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.ClientCascade);
        modelBuilder.Entity<ProgresoLectura>().HasOne(x => x.Usuario).WithMany(x => x.ProgresosLectura)
            .HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.ClientCascade);
        modelBuilder.Entity<Resena>().HasOne(x => x.Usuario).WithMany(x => x.Resenas)
            .HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.ClientCascade);
    }
}