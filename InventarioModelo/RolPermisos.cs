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
        public int Id { get; set; }
        [ForeignKey(nameof(Roles))]
        public int RolId { get; set; }
        public string Modulo { get; set; } = "";
        public DateTime Fecha { get; set; }
        public bool Ver { get; set; }
        public bool Agregar { get; set; }
        public bool Modificar { get; set; }
        public bool Eliminar { get; set; }
        public bool Activo { get; set; }


        public virtual Roles Roles { get; set; }
    }
}
