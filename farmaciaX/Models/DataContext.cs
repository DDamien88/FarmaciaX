using Microsoft.EntityFrameworkCore;

namespace farmaciaX.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Receta_Medica> Receta { get; set; }

        public DbSet<Ventas> Ventas { get; set; }
        public DbSet<DetalleVentas> DetalleVentas { get; set; }

        public DbSet<RecetaProductos> RecetaProductos { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Receta_Medica>()
                    .HasOne(r => r.Cliente)
                    .WithMany(c => c.Recetas)
                    .HasForeignKey(r => r.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Ventas>()
                .HasMany(v => v.VentaProductos)
                .WithOne()
                .HasForeignKey(d => d.Venta_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecetaProductos>()
                .HasKey(rp => new { rp.RecetaId, rp.ProductoId });


        }

    }
}