using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TiendaAlquiler.ViewModels
{
    public class RegisterViewModel
    {
        [DisplayName("Nombre de Usuario")]
        [Required(ErrorMessage = "Este campo es obligatorio y debe tener al menos tres letras")]
        [StringLength(50, MinimumLength = 3)]
        public required string UserName { get; set; } // Agregar la propiedad UsuarioNombre

        [DisplayName("Correo Electronico")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [EmailAddress]
        public required string Email { get; set; }

        [DisplayName("Introduzca la contraseña")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
        ErrorMessage = "La contraseña debe contener 8 caracteres, al menos una letra mayúscula," +
         " una letra minúscula, un número y un carácter especial.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [DisplayName("Repita la contraseña")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmPassword { get; set; }
    }
}
