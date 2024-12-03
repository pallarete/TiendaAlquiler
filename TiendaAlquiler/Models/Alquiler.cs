using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TiendaAlquiler.Models;

public partial class Alquiler
{
    [Key]
    public int AlquilerId { get; set; }
    [Required]
    public int CocheId { get; set; }
    [Required]
    public string UsuarioId { get; set; }
   
    [Display(Name ="Fecha de Inicio")]
    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    [DataType(DataType.Date)]
    public DateOnly FechaAlquiler { get; set; }
    
    [Display(Name = "Fecha de Fin")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "La fecha de devolucion es obligatoria")]
    public DateOnly FechaDevolucion { get; set; }

    [Display(Name = "Importe")]
    [Range(0, double.MaxValue, ErrorMessage = "El precio final debe ser un valor positivo.")]
    public decimal PrecioFinal { get; set; }

    // Campos para el pago simulado
    [NotMapped]
    [Required(ErrorMessage = "El número de tarjeta es obligatorio")]
    [StringLength(16, MinimumLength = 16, ErrorMessage = "El número de tarjeta debe tener 16 dígitos")]
    public string NumeroTarjeta { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "La fecha de expiración es obligatoria")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Formato inválido. Utilice MM/AA")]
    public string FechaExpiracion { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "El código CVC es obligatorio")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "El código CVC debe tener 3 dígitos")]
    public string CVC { get; set; }

    public virtual Coche Coche { get; set; }

    public virtual Usuario Usuario { get; set; }
    public Alquiler()
    {
        FechaAlquiler = DateOnly.FromDateTime(DateTime.Now);
        FechaDevolucion = DateOnly.FromDateTime(DateTime.Now);
    }
}