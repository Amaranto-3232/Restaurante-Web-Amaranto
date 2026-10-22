using Microsoft.EntityFrameworkCore;
using Restaurante.Models;


namespace Restaurante.Data
{
    public class RestauranteDbContext : DbContext
    {
        public RestauranteDbContext(
            DbContextOptions<RestauranteDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
    }
}