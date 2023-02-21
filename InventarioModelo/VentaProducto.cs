using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class VentaProducto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Producto_ID { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
        public int Usuario_ID { get; set; }
        public int Venta_ID { get; set; }

        public VentaProducto()
        {


            this.Cantidad = 0;
            this.Total = 0;
            this.Fecha = DateTime.Now;

        }
    }
}
