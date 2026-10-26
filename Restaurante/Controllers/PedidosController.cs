using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Extensions;
using Restaurante.Models;
using Restaurante.ViewModels;

namespace Restaurante.Controllers
{
    public class PedidosController : Controller
    {
        private readonly RestauranteDbContext _context;

        public PedidosController(RestauranteDbContext context)
        {
            _context = context;
        }

        // Listar pedidos, opcionalmente filtrados por estado
        public async Task<IActionResult> Index(EstadoPedido? estado)
        {
            var consulta = _context.Pedidos
                .Include(p => p.Mesa)
                .Include(p => p.Detalles)
                .AsQueryable();

            if (estado.HasValue)
                consulta = consulta.Where(p => p.Estado == estado.Value);

            var pedidos = await consulta
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            ViewBag.EstadoFiltro = estado;
            return View(pedidos);
        }

        // Ver un pedido con sus productos
        public async Task<IActionResult> Details(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Mesa)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.ID_Pedido == id);

            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        // Formulario de nuevo pedido
        public async Task<IActionResult> Create(int? mesa)
        {
            var modelo = new PedidoViewModel { ID_Mesa = mesa };
            modelo.Lineas.Add(new LineaPedidoViewModel());

            await CargarListas(modelo);
            return View(modelo);
        }

        // Guardar nuevo pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PedidoViewModel modelo)
        {
            // Unir productos repetidos y quitar filas vacías
            var lineas = modelo.Lineas
                .Where(l => l.ID_Producto > 0 && l.Cantidad > 0)
                .GroupBy(l => l.ID_Producto)
                .Select(g => new { ID_Producto = g.Key, Cantidad = g.Sum(l => l.Cantidad) })
                .ToList();

            if (!lineas.Any())
                ModelState.AddModelError(string.Empty, "Agrega al menos un producto al pedido.");

            var mesa = modelo.ID_Mesa.HasValue
                ? await _context.Mesas.FindAsync(modelo.ID_Mesa.Value)
                : null;

            if (modelo.ID_Mesa.HasValue && mesa == null)
                ModelState.AddModelError(nameof(modelo.ID_Mesa), "La mesa seleccionada no existe.");
            else if (mesa?.Estado == EstadoMesa.Ocupada)
                ModelState.AddModelError(nameof(modelo.ID_Mesa), $"La mesa {mesa.Numero} ya está ocupada.");

            var ids = lineas.Select(l => l.ID_Producto).ToList();
            var productos = await _context.Productos
                .Where(p => ids.Contains(p.ID_Producto) && p.Estado)
                .ToDictionaryAsync(p => p.ID_Producto);

            if (productos.Count != ids.Count)
                ModelState.AddModelError(string.Empty, "Algún producto ya no está disponible.");

            if (!ModelState.IsValid)
            {
                if (!modelo.Lineas.Any())
                    modelo.Lineas.Add(new LineaPedidoViewModel());

                await CargarListas(modelo);
                return View(modelo);
            }

            var pedido = new Pedido
            {
                ID_Mesa = mesa!.ID_Mesa,
                Cliente = string.IsNullOrWhiteSpace(modelo.Cliente) ? null : modelo.Cliente.Trim(),
                Observacion = string.IsNullOrWhiteSpace(modelo.Observacion) ? null : modelo.Observacion.Trim(),
                Fecha = DateTime.Now,
                Estado = EstadoPedido.Pendiente
            };

            foreach (var linea in lineas)
            {
                pedido.Detalles.Add(new DetallePedido
                {
                    ID_Producto = linea.ID_Producto,
                    Cantidad = linea.Cantidad,
                    PrecioUnitario = productos[linea.ID_Producto].Precio
                });
            }

            pedido.Total = pedido.Detalles.Sum(d => d.Subtotal);

            // La mesa queda ocupada mientras el pedido esté activo
            mesa.Estado = EstadoMesa.Ocupada;

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Pedido #{pedido.ID_Pedido} registrado para la mesa {mesa.Numero}.";
            return RedirectToAction(nameof(Details), new { id = pedido.ID_Pedido });
        }

        // Avanzar o cancelar un pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, EstadoPedido estado)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Mesa)
                .FirstOrDefaultAsync(p => p.ID_Pedido == id);

            if (pedido == null)
                return NotFound();

            if (EstaCerrado(pedido.Estado))
            {
                TempData["Error"] = "Este pedido ya está cerrado y no se puede modificar.";
                return RedirectToAction(nameof(Details), new { id });
            }

            pedido.Estado = estado;

            if (EstaCerrado(estado))
                await LiberarMesaSiCorresponde(pedido);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Pedido #{pedido.ID_Pedido} marcado como \"{estado.NombreVisible()}\".";
            return RedirectToAction(nameof(Details), new { id });
        }

        // Confirmar eliminación
        public async Task<IActionResult> Delete(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Mesa)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.ID_Pedido == id);

            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        // Eliminar pedido (sus detalles se borran en cascada)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Mesa)
                .FirstOrDefaultAsync(p => p.ID_Pedido == id);

            if (pedido != null)
            {
                if (!EstaCerrado(pedido.Estado))
                {
                    pedido.Estado = EstadoPedido.Cancelado;
                    await LiberarMesaSiCorresponde(pedido);
                }

                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = $"Pedido #{id} eliminado.";
            }

            return RedirectToAction(nameof(Index));
        }

        private static bool EstaCerrado(EstadoPedido estado) =>
            estado == EstadoPedido.Entregado || estado == EstadoPedido.Cancelado;

        // Libera la mesa si no le queda otro pedido activo
        private async Task LiberarMesaSiCorresponde(Pedido pedido)
        {
            if (pedido.Mesa == null)
                return;

            bool otroActivo = await _context.Pedidos.AnyAsync(p =>
                p.ID_Mesa == pedido.ID_Mesa &&
                p.ID_Pedido != pedido.ID_Pedido &&
                (p.Estado == EstadoPedido.Pendiente || p.Estado == EstadoPedido.EnPreparacion));

            if (!otroActivo)
                pedido.Mesa.Estado = EstadoMesa.Libre;
        }

        private async Task CargarListas(PedidoViewModel modelo)
        {
            var mesas = await _context.Mesas
                .Where(m => m.Estado != EstadoMesa.Ocupada || m.ID_Mesa == modelo.ID_Mesa)
                .OrderBy(m => m.Numero)
                .ToListAsync();

            modelo.Mesas = mesas.Select(m => new SelectListItem(
                $"Mesa {m.Numero} · {m.Capacidad} personas" +
                    (m.Estado == EstadoMesa.Reservada ? " (reservada)" : ""),
                m.ID_Mesa.ToString(),
                m.ID_Mesa == modelo.ID_Mesa));

            modelo.Productos = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Estado)
                .OrderBy(p => p.Categoria!.Nombre)
                .ThenBy(p => p.Nombre)
                .Select(p => new ProductoOpcion
                {
                    Id = p.ID_Producto,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria != null ? p.Categoria.Nombre : "Sin categoría",
                    Precio = p.Precio
                })
                .ToListAsync();

            modelo.Clientes = ClientesDatosPrueba.Clientes
                .Select(c => $"{c.Nombre} {c.Apellido}")
                .OrderBy(n => n);
        }
    }
}
