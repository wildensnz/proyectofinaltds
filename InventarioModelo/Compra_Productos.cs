using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Compra_Productos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }
        public int CompraId { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Productos Productos { get; set; } = new Productos();

        [ForeignKey("UsuarioId")]
        public virtual Usuarios Usuarios { get; set; } = new Usuarios();

        [ForeignKey("CompraId")]
        public virtual Compras Compras { get; set; } = new Compras();
    }
}
