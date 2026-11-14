using System.ComponentModel.DataAnnotations;


namespace Flash_Lector.Models;

public class SolicitudRol
{
    public int Id { get; set; }

    public int UsuarioId { get; set; } 
    public Usuario Usuario { get; set; } = null!;

    [Required, MaxLength(40)]
    public string RolSolicitado { get; set; } = "Creador";

    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

    [MaxLength(500)]
    public string? NotaAdministrador { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}