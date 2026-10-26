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
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // No puede haber dos mesas con el mismo número
            modelBuilder.Entity<Mesa>()
                .HasIndex(m => m.Numero)
                .IsUnique();

            // Una mesa con pedidos no se puede borrar
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Mesa)
                .WithMany(m => m.Pedidos)
                .HasForeignKey(p => p.ID_Mesa)
                .OnDelete(DeleteBehavior.Restrict);

            // Al borrar un pedido se borran sus detalles
            modelBuilder.Entity<DetallePedido>()
                .HasOne(d => d.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(d => d.ID_Pedido)
                .OnDelete(DeleteBehavior.Cascade);

            // Un producto usado en pedidos no se puede borrar
            modelBuilder.Entity<DetallePedido>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ID_Producto)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
