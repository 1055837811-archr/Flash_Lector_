using Flash_Lector.Models;
using System.ComponentModel.DataAnnotations;


namespace Flash_Lector.ViewModels;

public class ObraFormularioViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(180)]
    public string Titulo { get; set; } = "";

    [Required, MaxLength(120)]
    public string NombreAutor { get; set; } = "";

    [Required, MaxLength(1600)]
    public string Sinopsis { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int Paginas { get; set; }

    public FormatoObra Formato { get; set; }
    public int CategoriaId { get; set; }
    public IFormFile? Archivo { get; set; }
}