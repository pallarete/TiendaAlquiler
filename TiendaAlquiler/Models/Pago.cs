
using System.ComponentModel.DataAnnotations;

namespace TiendaAlquiler.Models
{
    public class Pago
    {

        [Required]
        [Display(Name = "Número de Tarjeta")]
        [CreditCard] // Validación para asegurarse de que el formato de la tarjeta sea válido
        public string  NumeroTarjeta { get; set; }

        [Required]
        [Display(Name = "Nombre del Titular")]
        public string NombreTitular { get; set; }

        [Required]
        [Display(Name = "Fecha de Vencimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Required]
        [Display(Name = "Código de Seguridad")]
        [Range(100, 999, ErrorMessage = "El código de seguridad debe ser de 3 dígitos")]
        public int CodigoSeguridad { get; set; }

        [Required]
        [Display(Name = "Monto")]
        [Range(1, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Monto { get; set; }

    }
}
