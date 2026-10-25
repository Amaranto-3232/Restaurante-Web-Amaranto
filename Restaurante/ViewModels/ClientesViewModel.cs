using Restaurante.Models;

namespace Restaurante.ViewModels
{
    public class ClientesViewModel
    {
        public List<Cliente> Clientes { get; set; } = new();
        public string Busqueda { get; set; } = string.Empty;
        public int PaginaActual { get; set; } = 1;
        public int TotalClientes { get; set; }
        public int ClientesPorPagina => 10;

        public int TotalPaginas =>
            Math.Max(1, (int)Math.Ceiling(
                TotalClientes / (double)ClientesPorPagina));

        public bool TieneAnterior => PaginaActual > 1;
        public bool TieneSiguiente => PaginaActual < TotalPaginas;

        public int Desde => TotalClientes == 0
            ? 0
            : (PaginaActual - 1) * ClientesPorPagina + 1;

        public int Hasta =>
            Math.Min(PaginaActual * ClientesPorPagina, TotalClientes);
    }
}