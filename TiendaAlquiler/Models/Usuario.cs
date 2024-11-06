using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace TiendaAlquiler.Models;

public partial class Usuario : IdentityUser
{
    //[Required]
    public string UsuarioNombre { get; set; }

    //[Required]
    public string Rol { get; set; }

    public virtual ICollection<Alquiler> Alquilers { get; set; } = new List<Alquiler>();
}