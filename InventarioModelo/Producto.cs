using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Modelo { get; set; }
        public string Presentacion { get; set; }
        public string Marca { get; set; }
        public decimal Costo { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Stock { get; set; }
        public bool Estatus { get; set; }
        public int Empresa_ID { get; set; }
        public int? Almacen_ID { get; set; }
        public int Usuario_ID { get; set; }

        public Producto()
        {

            this.Codigo = "";
            this.Nombre = "";
            this.Descripcion = "";
            this.Modelo = "";
            this.Presentacion = "";
            this.Marca = "";
            this.Costo = 0;
            this.PrecioVenta = 0;
            this.Stock = 0;
            this.Estatus = false;

        }
    }
}
