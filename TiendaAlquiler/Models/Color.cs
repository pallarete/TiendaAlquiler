#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaAlquiler.Models;

public partial class Color
{
    [Key]
    public int ColorId { get; set; }

    [Required(ErrorMessage = "El nombre del color es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre del color no puede tener más de 200 caracteres.")]
    [RegularExpression(@"^[A-Za-zÁáÉéÍíÓóÚúÑñ\s]+$", ErrorMessage = "El tipo de color solo puede contener letras y espacios.")]

    public string Nombre { get; set; }

    public virtual ICollection<Coche> Coches { get; set; } = new List<Coche>();
}