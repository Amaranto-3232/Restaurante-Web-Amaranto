using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurante.Models
{
    public enum EstadoPedido
    {
        Pendiente = 0,
        [Display(Name = "En preparación")]
        EnPreparacion = 1,
        Entregado = 2,
        Cancelado = 3
    }

    public class Pedido
    {
        [Key]
        public int ID_Pedido { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Debes seleccionar una mesa.")]
        [Display(Name = "Mesa")]
        public int ID_Mesa { get; set; }

        [ForeignKey(nameof(ID_Mesa))]
        public Mesa? Mesa { get; set; }

        [StringLength(150)]
        [Display(Name = "Cliente")]
        public string? Cliente { get; set; }

        [StringLength(250)]
        [Display(Name = "Observación")]
        public string? Observacion { get; set; }

        [Display(Name = "Estado")]
        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Total (Bs)")]
        public decimal Total { get; set; }

        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
