using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Usuarios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public int RolId { get; set; }
        public string Password { get; set; } = "";
        public bool Estatus { get; set; }
        public int? UsuarioPadreId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        public virtual IEnumerable<Compra_Productos> Compra_Productos { get; set; } = new List<Compra_Productos>();
        public virtual IEnumerable<Compras> Compras { get; set; } = new List<Compras>();
        public virtual IEnumerable<Contactos> Contactos { get; set; } = new List<Contactos>();
        public virtual IEnumerable<Empresas> Empresas { get; set; } = new List<Empresas>();
        public virtual IEnumerable<Productos> Productos { get; set; } = new List<Productos>();
        public virtual IEnumerable<Tokens> Tokens { get; set; } = new List<Tokens>();

        [ForeignKey("RolId")]
        public virtual Roles Roles { get; set; } = new Roles();

        public virtual IEnumerable<Venta_Productos> Venta_Productos { get; set; } = new List<Venta_Productos>();
        public virtual IEnumerable<Ventas> Ventas { get; set; } = new List<Ventas>();
    }
    
}
