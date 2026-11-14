using System.ComponentModel.DataAnnotations;

namespace Flash_Lector.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Nombres { get; set; } = "";

    [Required, MaxLength(80)]
    public string Apellidos { get; set; } = "";

    [Required, EmailAddress, MaxLength(150)]
    public string Correo { get; set; } = "";

    [Required]
    public string ContrasenaHash { get; set; } = "";

    [Required, MaxLength(40)]
    public string Rol { get; set; } = "Lector";

    public ICollection<Obra> Obras { get; set; } = [];
    public ICollection<Favorito> Favoritos { get; set; } = [];
    public ICollection<ProgresoLectura> ProgresosLectura { get; set; } = [];
    public ICollection<Resena> Resenas { get; set; } = [];
}