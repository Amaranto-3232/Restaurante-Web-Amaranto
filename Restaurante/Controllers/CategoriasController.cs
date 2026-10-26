
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurante.Data;
using Restaurante.Models;

namespace Restaurante.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly RestauranteDbContext _context;

        public CategoriasController(RestauranteDbContext context)
        {
            _context = context;
        }

        // Listar categorías
        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return View(categorias);
        }

        // Mostrar formulario de creación
        public IActionResult Create()
        {
            return View(new Categoria());
        }

        // Guardar categoría
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (!ModelState.IsValid)
                return View(categoria);

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Mostrar formulario de edición
        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // Guardar cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.ID_Categoria)
                return NotFound();

            if (!ModelState.IsValid)
                return View(categoria);

            _context.Update(categoria);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Mostrar detalles
        public async Task<IActionResult> Details(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.ID_Categoria == id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // Confirmar eliminación
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // Eliminar categoría
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
