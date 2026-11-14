using System.ComponentModel.DataAnnotations;

namespace Flash_Lector.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Nombre { get; set; } = "";

    public ICollection<Obra> Obras { get; set; } = [];
}