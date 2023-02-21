using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Roles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public DateTime Fecha { get; set; }
        public int Usuario_ID { get; set; }

        public Roles()
        {

            this.Rol = "";
            this.Activo = false;
            this.Fecha = DateTime.Now;

        }
    }
}
