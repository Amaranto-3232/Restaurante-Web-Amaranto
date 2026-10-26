using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models;
using Restaurante.ViewModels;
using System.Diagnostics;

namespace Restaurante.Controllers
{
    public class HomeController : Controller
    {
        private readonly RestauranteDbContext _context;

        public HomeController(RestauranteDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var manana = hoy.AddDays(1);

            var entregadosHoy = _context.Pedidos.Where(p =>
                p.Estado == EstadoPedido.Entregado &&
                p.Fecha >= hoy && p.Fecha < manana);

            // Producto con más unidades vendidas (sin contar pedidos cancelados)
            var masVendido = await _context.DetallesPedido
                .Where(d => d.Pedido!.Estado != EstadoPedido.Cancelado)
                .GroupBy(d => d.Producto!.Nombre)
                .Select(g => new { Nombre = g.Key, Cantidad = g.Sum(d => d.Cantidad) })
                .OrderByDescending(x => x.Cantidad)
                .FirstOrDefaultAsync();

            var modelo = new DashboardViewModel
            {
                VentasHoy = await entregadosHoy.SumAsync(p => p.Total),
                PedidosEntregadosHoy = await entregadosHoy.CountAsync(),
                PedidosActivos = await _context.Pedidos.CountAsync(p =>
                    p.Estado == EstadoPedido.Pendiente ||
                    p.Estado == EstadoPedido.EnPreparacion),
                MesasOcupadas = await _context.Mesas.CountAsync(m => m.Estado == EstadoMesa.Ocupada),
                MesasTotal = await _context.Mesas.CountAsync(),
                ProductosActivos = await _context.Productos.CountAsync(p => p.Estado),
                ProductoMasVendido = masVendido?.Nombre,
                ProductoMasVendidoCantidad = masVendido?.Cantidad ?? 0,
                UltimosPedidos = await _context.Pedidos
                    .Include(p => p.Mesa)
                    .OrderByDescending(p => p.Fecha)
                    .Take(5)
                    .ToListAsync()
            };

            return View(modelo);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
