using System.ComponentModel.DataAnnotations;

namespace Restaurante.Models
{
    public enum EstadoMesa
    {
        Libre = 0,
        Ocupada = 1,
        Reservada = 2
    }

    public class Mesa
    {
        [Key]
        public int ID_Mesa { get; set; }

        [Required(ErrorMessage = "El número de mesa es obligatorio.")]
        [Range(1, 999, ErrorMessage = "El número debe estar entre 1 y 999.")]
        [Display(Name = "Número de mesa")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "La capacidad es obligatoria.")]
        [Range(1, 20, ErrorMessage = "La capacidad debe estar entre 1 y 20 personas.")]
        [Display(Name = "Capacidad (personas)")]
        public int Capacidad { get; set; } = 4;

        [StringLength(50)]
        [Display(Name = "Ubicación")]
        public string? Ubicacion { get; set; }

        [Display(Name = "Estado")]
        public EstadoMesa Estado { get; set; } = EstadoMesa.Libre;

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
