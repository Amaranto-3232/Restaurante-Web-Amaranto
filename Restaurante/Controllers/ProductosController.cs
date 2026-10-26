
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models;

namespace Restaurante.Controllers
{
    public class ProductosController : Controller
    {
        private readonly RestauranteDbContext _context;

        public ProductosController(RestauranteDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .ToListAsync();

            return View(productos);
        }

        public async Task<IActionResult> Create()
        {
            await CargarCategorias();
            return View(new Producto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (!await _context.Categorias.AnyAsync(
                c => c.ID_Categoria == producto.ID_Categoria && c.Estado))
            {
                ModelState.AddModelError(
                    "ID_Categoria", "Selecciona una categoría activa.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await CargarCategorias(producto.ID_Categoria);
            return View(producto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound();

            await CargarCategorias(producto.ID_Categoria);
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.ID_Producto)
                return NotFound();

            if (!await _context.Categorias.AnyAsync(
                c => c.ID_Categoria == producto.ID_Categoria && c.Estado))
            {
                ModelState.AddModelError(
                    "ID_Categoria", "Selecciona una categoría activa.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await CargarCategorias(producto.ID_Categoria);
            return View(producto);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.ID_Producto == id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.ID_Producto == id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto != null)
            {
                // Si ya se vendió en algún pedido, solo se desactiva
                if (await _context.DetallesPedido.AnyAsync(d => d.ID_Producto == id))
                {
                    producto.Estado = false;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCategorias(int? categoriaSeleccionada = null)
        {
            var categorias = await _context.Categorias
                .Where(c => c.Estado)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.ID_Categoria = new SelectList(
                categorias,
                "ID_Categoria",
                "Nombre",
                categoriaSeleccionada);
        }
    }
}
