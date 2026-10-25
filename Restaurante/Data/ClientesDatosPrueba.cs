using Bogus;
using Restaurante.Models;

namespace Restaurante.Data
{
    public static class ClientesDatosPrueba
    {
        private static readonly Lazy<List<Cliente>> datos =
    new Lazy<List<Cliente>>(GenerarClientes);

        public static IReadOnlyList<Cliente> Clientes
        {
            get
            {
                var lista = datos.Value;

                lock (lista)
                {
                    return lista.ToList();
                }
            }
        }

        public static bool Eliminar(int id)
        {
            var lista = datos.Value;

            lock (lista)
            {
                var cliente = lista.FirstOrDefault(c => c.Id == id);

                if (cliente == null)
                {
                    return false;
                }

                return lista.Remove(cliente);
            }
        }

        private static List<Cliente> GenerarClientes()
        {
            var apellidos = new[]
            {
                "Vedia", "Tellez", "Flores", "maizares",
                "Vargas", "Rojas", "Pérez", "López",
                "Gutiérrez", "Fernández", "García", "Cruz",
                "Quiroga", "Mendoza", "Aguilar", "Suárez",
                "Torrez", "Arce", "Salazar", "Romero"
            };

            int id = 1;

            var generador = new Faker<Cliente>("es")
                .UseSeed(12345)
                .RuleFor(c => c.Id, f => id++)
                .RuleFor(c => c.Nombre, f => f.Name.FirstName())
                .RuleFor(c => c.Apellido, f =>
                    $"{f.PickRandom(apellidos)} {f.PickRandom(apellidos)}")
                .RuleFor(c => c.Telefono, f =>
                    f.Random.Replace("########")
                        .Remove(0, 1)
                        .Insert(0, f.PickRandom("6", "7")))
                .RuleFor(c => c.Correo, (f, c) =>
                    f.Internet.Email(
                        c.Nombre,
                        c.Apellido.Replace(" ", ""),
                        "gmail.com",
                        c.Id.ToString()));

            return generador.Generate(100);
        }
    }
}