using System.ComponentModel.DataAnnotations;

namespace Restaurante.Models
{
    public class Categoria
    {
        [Key]
        public int ID_Categoria { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;
    }
}