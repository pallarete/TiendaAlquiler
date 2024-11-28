using Microsoft.AspNetCore.Identity;


namespace TiendaAlquiler.Models;

public partial class Usuario : IdentityUser
{
    public virtual ICollection<Alquiler> Alquilers { get; set; } = new List<Alquiler>();
}