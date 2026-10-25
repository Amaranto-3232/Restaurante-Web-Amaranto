using Microsoft.AspNetCore.Mvc;
using Restaurante.Models;
using Restaurante.ViewModels;
using Restaurante.Data;

namespace Restaurante.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Index(string? busqueda, int pagina = 1)
        {
            var clientes = ClientesDatosPrueba.Clientes;

            string filtro = busqueda?.Trim() ?? string.Empty;

            IEnumerable<Cliente> consulta = clientes;

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                consulta = consulta.Where(c =>
                    c.Nombre.Contains(
                        filtro, StringComparison.OrdinalIgnoreCase) ||
                    c.Apellido.Contains(
                        filtro, StringComparison.OrdinalIgnoreCase));
            }

            var modelo = new ClientesViewModel
            {
                Busqueda = filtro,
                TotalClientes = consulta.Count()
            };

            modelo.PaginaActual = Math.Clamp(
                pagina, 1, modelo.TotalPaginas);

            modelo.Clientes = consulta
                .OrderBy(c => c.Id)
                .Skip((modelo.PaginaActual - 1) * modelo.ClientesPorPagina)
                .Take(modelo.ClientesPorPagina)
                .ToList();

            return View(modelo);
        }
        public IActionResult Detalles(int id)
        {
            var cliente = ClientesDatosPrueba.Clientes
                .FirstOrDefault(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var cliente = ClientesDatosPrueba.Clientes
                .FirstOrDefault(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            var modelo = new Cliente
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Telefono = cliente.Telefono,
                Correo = cliente.Correo
            };

            return View(modelo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(
    int id,
    [Bind("Id,Nombre,Apellido,Telefono,Correo")] Cliente modelo)
        {
            if (id != modelo.Id)
            {
                return BadRequest();
            }

            var cliente = ClientesDatosPrueba.Clientes
                .FirstOrDefault(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            cliente.Nombre = modelo.Nombre.Trim();
            cliente.Apellido = modelo.Apellido.Trim();
            cliente.Telefono = modelo.Telefono.Trim();
            cliente.Correo = modelo.Correo.Trim();

            TempData["Mensaje"] = "Cliente actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var cliente = ClientesDatosPrueba.Clientes
                .FirstOrDefault(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            bool eliminado = ClientesDatosPrueba.Eliminar(id);

            if (!eliminado)
            {
                return NotFound();
            }

            TempData["Mensaje"] = "Cliente eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}