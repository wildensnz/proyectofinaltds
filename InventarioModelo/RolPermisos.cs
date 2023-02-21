using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class RolPermisos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Accion { get; set; }
        public int Rol_ID { get; set; }
        public bool Activo { get; set; }
        public DateTime Fecha { get; set; }

        [ForeignKey("Rol_ID")]
        public virtual Roles Roles { get; set; }

        public RolPermisos()
        {

            this.Accion = "";
            this.Activo = false;
            this.Fecha = DateTime.Now;
            this.Roles = new Roles();
        }
    }
}
