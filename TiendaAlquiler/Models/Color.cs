﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
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
    [StringLength(50, ErrorMessage = "El nombre del color no puede tener más de 50 caracteres.")]
    public string Nombre { get; set; }

    public virtual ICollection<Coche> Coches { get; set; } = new List<Coche>();
}