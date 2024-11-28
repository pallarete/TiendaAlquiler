using System.ComponentModel.DataAnnotations;

namespace TiendaAlquiler.ViewModels
{
    public class UsuarioViewModel
    {
        public string? Id { get; set; }

        [Required]
        [RegularExpression(@"^(Admin|User)$", ErrorMessage = "Rol debe ser 'Admin' o 'User'")]
        public required string Rol { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmPassword { get; set; }
    }
}
