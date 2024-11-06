using System.ComponentModel.DataAnnotations;

namespace TiendaAlquiler.ViewModels
{
    public class UsuarioViewModel
    {
        public string? Id { get; set; }

        [Required]
        public string UsuarioNombre { get; set; }

        [Required]
        [RegularExpression(@"^(Administrador|Cliente)$", ErrorMessage = "Rol debe ser 'Administrador' o 'Cliente'")]
        public string Rol { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
