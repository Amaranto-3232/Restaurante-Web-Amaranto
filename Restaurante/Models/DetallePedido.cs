using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurante.Models
{
    public class DetallePedido
    {
        [Key]
        public int ID_Detalle { get; set; }

        public int ID_Pedido { get; set; }

        [ForeignKey(nameof(ID_Pedido))]
        public Pedido? Pedido { get; set; }

        [Display(Name = "Producto")]
        public int ID_Producto { get; set; }

        [ForeignKey(nameof(ID_Producto))]
        public Producto? Producto { get; set; }

        [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100.")]
        public int Cantidad { get; set; }

        // Se guarda el precio del momento por si luego cambia el del producto
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Precio unitario")]
        public decimal PrecioUnitario { get; set; }

        [NotMapped]
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
