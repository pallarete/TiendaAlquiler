using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TiendaAlquiler.Models;

public partial class Coche
{
    [Key]
    public int CocheId { get; set; }

    [DisplayName("Marca")]
    [Required(ErrorMessage = "Introduzca una Marca")]
    public required string Marca { get; set; }

    [Required(ErrorMessage = "Introduzca un Modelo")]
    public required string Modelo { get; set; }

    [DisplayName("Año")]
    [Required(ErrorMessage = "Introduzca un año de fabricacion")]
    public required int AnioFabricacion { get; set; }

    [DisplayName("Precio por dia")]
    [Required(ErrorMessage = "El precio por dia es obligatorio")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El precio de alquiler debe ser mayor a cero.")]
    [Column(TypeName = "decimal(18,2)")]
    public required decimal PrecioAlquiler { get; set; }

    public bool EstaAlquilado { get; set; }

    [DisplayName("Colores Disponibles")]
    [Required]
    public int ColorId { get; set; }

    [DisplayName("Tipos de Carrocerias")]
    [Required]
    public int CarroceriaId { get; set; }

    [DisplayName("Decada de Fabricacion")]
    [Required]
    public int DecadaId { get; set; }

    [DisplayName("Pais de Origen")]
    [Required]
    public int PaisId { get; set; }

    [DisplayName("Descripcion")]
    public string? Description { get; set; }

    [Required]
    public virtual ICollection<Alquiler> Alquilers { get; set; } = new List<Alquiler>();

    public virtual Carroceria? Carroceria { get; set; }
    public virtual Color? Color { get; set; }
    public virtual Decada? Decada { get; set; }
    public virtual Pais? Pais { get; set; }
    public virtual ICollection<Foto> Fotos { get; set; } = new List<Foto>();

}