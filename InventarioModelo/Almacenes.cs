using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Almacenes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public int Empresa_ID { get; set; }
        public bool Estatus { get; set; }
        public DateTime Fecha { get; set; }

        public Almacenes()
        {
            this.Nombre = "";
            this.Direccion = "";
            this.Estatus = false;
            this.Fecha = DateTime.Now;
        }
    }
}
