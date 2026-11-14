using System.ComponentModel.DataAnnotations;


namespace Flash_Lector.Models;

public class Obra
{
    public int Id { get; set; }

    [Required, MaxLength(180)]
    public string Titulo { get; set; } = "";

    [Required, MaxLength(120)]
    public string NombreAutor { get; set; } = "";

    [MaxLength(1600)]
    public string Sinopsis { get; set; } = "";

    public FormatoObra Formato { get; set; }
    public EstadoPublicacion Estado { get; set; } = EstadoPublicacion.EnRevision;
    public int Paginas { get; set; }

    [MaxLength(260)]
    public string? RutaArchivo { get; set; }

    public DateTime? FechaPublicacion { get; set; }

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public int? CreadorId { get; set; }
    public Usuario? Creador { get; set; }

    public ICollection<Favorito> Favoritos { get; set; } = [];
    public ICollection<ProgresoLectura> ProgresosLectura { get; set; } = [];
    public ICollection<Resena> Resenas { get; set; } = [];
}