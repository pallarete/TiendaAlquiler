using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TiendaAlquiler.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("Nombre")]
        [Required(ErrorMessage = "Nombre obligatorio")]
        public required string UsuarioNombre { get; set; }

        [DisplayName("Contraseña")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
        ErrorMessage = "La contraseña debe contener al menos una letra mayúscula," +
         " una letra minúscula, un número y un carácter especial.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
