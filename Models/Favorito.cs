namespace Flash_Lector.Models;

public class Favorito
{
    public int UsuarioId { get; set; } 
    public Usuario Usuario { get; set; } = null!;

    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}