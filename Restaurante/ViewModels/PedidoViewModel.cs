using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Restaurante.ViewModels
{
    // Datos del formulario "Nuevo pedido"
    public class PedidoViewModel
    {
        [Required(ErrorMessage = "Debes seleccionar una mesa.")]
        [Display(Name = "Mesa")]
        public int? ID_Mesa { get; set; }

        [StringLength(150)]
        [Display(Name = "Cliente (opcional)")]
        public string? Cliente { get; set; }

        [StringLength(250)]
        [Display(Name = "Observación")]
        public string? Observacion { get; set; }

        public List<LineaPedidoViewModel> Lineas { get; set; } = new();

        // Listas para los <select> y el cálculo del total en pantalla
        public IEnumerable<SelectListItem> Mesas { get; set; } = Enumerable.Empty<SelectListItem>();
        public List<ProductoOpcion> Productos { get; set; } = new();
        public IEnumerable<string> Clientes { get; set; } = Enumerable.Empty<string>();
    }

    public class LineaPedidoViewModel
    {
        public int ID_Producto { get; set; }

        [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100.")]
        public int Cantidad { get; set; } = 1;
    }

    public class ProductoOpcion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal Precio { get; set; }
    }
}
