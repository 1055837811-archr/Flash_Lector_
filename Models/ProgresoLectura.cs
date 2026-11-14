namespace Flash_Lector.Models;

public class ProgresoLectura
{
    public int UsuarioId { get; set; } 
    public Usuario Usuario { get; set; } = null!;

    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public int PaginaActual { get; set; }
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
}