using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Flash_Lector.Data;

public class FabricaBaseDatosContext : IDesignTimeDbContextFactory<ContextBaseDatos>
{
    public ContextBaseDatos CreateDbContext(string[] args)
    {
        var opciones = new DbContextOptionsBuilder<ContextBaseDatos>()
            .UseSqlServer("Server=LISBETH\\SQLEXPRESS;Database=BibliotecaWebDb;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;
        return new ContextBaseDatos(opciones);
    }
}