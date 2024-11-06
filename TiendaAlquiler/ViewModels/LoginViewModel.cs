using System.ComponentModel.DataAnnotations;

namespace TiendaAlquiler.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UsuarioNombre { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
