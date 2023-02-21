using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Empresas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Nombre { get; set; }
        public bool Estatus { get; set; }
        public DateTime Fecha { get; set; }
        public int Usuario_ID { get; set; }

        public Empresas()
        {

            this.Nombre = "";
            this.Estatus = false;
            this.Fecha = DateTime.Now;
        }
    }
}
