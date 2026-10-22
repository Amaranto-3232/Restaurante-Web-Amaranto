
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurante.Models
{
    public class Producto
    {
        [Key]
        public int ID_Producto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Nombre del producto")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor que cero.")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Precio (Bs)")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una categoría.")]
        [Display(Name = "Categoría")]
        public int ID_Categoria { get; set; }

        [ForeignKey(nameof(ID_Categoria))]
        public Categoria? Categoria { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;
    }
}
