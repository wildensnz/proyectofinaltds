using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Productos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Modelo { get; set; } = "";
        public string Presentacion { get; set; } = "";
        public string Marca { get; set; } = "";
        public decimal Costo { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Stock { get; set; }
        public bool Estatus { get; set; }
        public int EmpresaId { get; set; }
        public int UsuarioId { get; set; }
        public int? ArchivoId { get; set; }
        public int Impuesto { get; set; }



        public virtual IEnumerable<Compra_Productos> Compra_Productos { get; set; } = new List<Compra_Productos>();


        [ForeignKey("EmpresaId")]
        public virtual Empresas Empresas { get; set; } = new Empresas();

        [ForeignKey("ArchivoId")]
        public virtual Archivos? Archivos { get; set; } = new Archivos();

        [ForeignKey("UsuarioId")]
        public virtual Usuarios Usuarios { get; set; } = new Usuarios();
        public virtual IEnumerable<Venta_Productos> Venta_Productos { get; set; } = new List<Venta_Productos>();
    }
}
