using Restaurante.Models;

namespace Restaurante.ViewModels
{
    // Indicadores que se muestran en el Dashboard
    public class DashboardViewModel
    {
        public decimal VentasHoy { get; set; }
        public int PedidosEntregadosHoy { get; set; }
        public int PedidosActivos { get; set; }
        public int MesasOcupadas { get; set; }
        public int MesasTotal { get; set; }
        public int ProductosActivos { get; set; }

        public string? ProductoMasVendido { get; set; }
        public int ProductoMasVendidoCantidad { get; set; }

        public List<Pedido> UltimosPedidos { get; set; } = new();
    }
}
