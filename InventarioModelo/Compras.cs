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
        public int ID { get; set; }
        public int Proveedor_ID { get; set; }
        public DateTime Fecha { get; set; }
        public string Codigo { get; set; }
        public decimal Total { get; set; }
        public bool Estatus { get; set; }
        public int Empresa_ID { get; set; }
        public int Usuario_ID { get; set; }

        public Compras()
        {

            this.Fecha = DateTime.Now;
            this.Codigo = "";
            this.Total = 0;
            this.Estatus = false;

        }
    }
}
