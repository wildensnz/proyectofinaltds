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

        public DbSet<Almacenes> Almacenes { get; set; }
        public DbSet<CompraProducto> CompraProducto { get; set; }
        public DbSet<Compras> Compras { get; set; }
        public DbSet<Contactos> Contactos { get; set; }
        public DbSet<Empresas> Empresas { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<RolPermisos> RolPermisos { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<VentaProducto> VentaProducto { get; set; }
        public DbSet<Ventas> Ventas { get; set; }

    }
}
