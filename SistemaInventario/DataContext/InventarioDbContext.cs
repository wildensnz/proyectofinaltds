using InventarioModelo;
using Microsoft.EntityFrameworkCore;

namespace SistemaInventario.DataContext
{
    public class InventarioDbContext : DbContext
    {
        public InventarioDbContext(DbContextOptions<InventarioDbContext> options)
        : base(options)
        {
        }

        public DbSet<Compra_Productos> Compra_Productos { get; set; }
        public DbSet<Compras> Compras { get; set; }
        public DbSet<Contactos> Contactos { get; set; }
        public DbSet<Empresas> Empresas { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Venta_Productos> Venta_Productos { get; set; }
        public DbSet<Ventas> Ventas { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<RolPermisos> RolPermisos { get; set; }
        public DbSet<Tokens> Tokens { get; set; }
        public DbSet<Monedas> Monedas { get; set; }
        public DbSet<Impuestos> Impuestos { get; set; }
        public DbSet<Catalogos> Catalogos { get; set; }
        public DbSet<Archivos> Archivos { get; set; }


    }
}
