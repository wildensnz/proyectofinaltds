using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Ventas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime Fecha { get; set; }
        public int Contacto_ID { get; set; }
        public string CodigoFactura { get; set; }
        public decimal Total { get; set; }
        public bool Estatus { get; set; }
        public int Empresa_ID { get; set; }
        public int Usuario_ID { get; set; }

        public Ventas()
        {

            this.Fecha = DateTime.Now;
            this.CodigoFactura = "";
            this.Total = 0;
            this.Estatus = false;

        }
    }
}
