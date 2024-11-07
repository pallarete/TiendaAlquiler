using System.ComponentModel.DataAnnotations;

namespace TiendaAlquiler.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; } // Agregar la propiedad UsuarioNombre

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
