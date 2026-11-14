using System.ComponentModel.DataAnnotations;

namespace Flash_Lector.ViewModels;

public class RegistroViewModel
{
    [Required]
    public string Nombres { get; set; } = "";

    [Required]
    public string Apellidos { get; set; } = "";

    [Required, EmailAddress]
    public string Correo { get; set; } = "";

    [Required, MinLength(8), DataType(DataType.Password)]
    public string Contrasena { get; set; } = "";
}