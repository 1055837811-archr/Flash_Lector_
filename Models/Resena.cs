using System.ComponentModel.DataAnnotations;

namespace Flash_Lector.Models;

public class Resena
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;

    [Range(1, 5)]
    public int Calificacion { get; set; }

    [Required, MaxLength(1000)]
    public string Comentario { get; set; } = "";

    public bool EsVisible { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}