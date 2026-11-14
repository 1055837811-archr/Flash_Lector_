using System.ComponentModel.DataAnnotations;

namespace Flash_Lector.ViewModels;

public class InicioSesionViewModel
{
    [Required, EmailAddress]
    public string Correo { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Contrasena { get; set; } = "";

    public bool Recordarme { get; set; }
}