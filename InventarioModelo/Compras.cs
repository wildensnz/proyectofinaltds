using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Compras
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaCompra { get; set; }
        public string CodigoFactura { get; set; } = "";
        public decimal Neto { get; set; }
        public decimal PorcentajeImpuesto { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public bool Estatus { get; set; }
        public int EmpresaId { get; set; }
        public int UsuarioId { get; set; }



        [ForeignKey("ProveedorId")]
        public virtual Contactos Contactos { get; set; } = new Contactos();

        [ForeignKey("EmpresaId")]
        public virtual Empresas Empresas { get; set; } = new Empresas();

        [ForeignKey("UsuarioId")]
        public virtual Usuarios Usuarios { get; set; } = new Usuarios();

        public virtual IEnumerable<Compra_Productos> Compra_Productos { get; set; } = new List<Compra_Productos>();
    }
}
