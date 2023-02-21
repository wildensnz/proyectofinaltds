using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Usuarios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public int Rol_ID { get; set; }
        public bool Estatus { get; set; }
        public string Clave { get; set; }
        public int? UsuarioPadreID { get; set; }

        [ForeignKey("Rol_ID")]
        public virtual Roles Roles { get; set; }

        public Usuarios()
        {
            this.Nombre = "";
            this.Email = "";
            this.Roles = new Roles();
            this.Estatus = false;
            this.Clave = "";
        }
    }
}
