using System.ComponentModel.DataAnnotations;

namespace MediGrids.Models
{
    /// <summary>
    /// ViewModel para Gestion de Terapeutas - RF-09
    /// </summary>
    public class GestionTerapeutaViewModel
    {
        public int IdTerapeuta { get; set; }
        public int? IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Display(Name = "Telefono")]
        public string Telefono { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email no valido")]
        public string Email { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        [Display(Name = "Especialidad (Categoria)")]
        public int? IdCategoria { get; set; }

        public string NombreCategoria { get; set; }
        public string NombreTipoTerapia { get; set; }
        public string Username { get; set; }
    }

    /// <summary>
    /// ViewModel para registro de terapeuta - incluye credenciales de usuario
    /// </summary>
    public class RegistroTerapeutaViewModel : GestionTerapeutaViewModel
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        [Display(Name = "Usuario (para login)")]
        public string UsernameRegistro { get; set; }

        [Required(ErrorMessage = "La contrasena es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contrasena")]
        [MinLength(4, ErrorMessage = "Minimo 4 caracteres")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contrasena")]
        [Compare("Password", ErrorMessage = "Las contrasenas no coinciden")]
        public string ConfirmarPassword { get; set; }
    }
}
