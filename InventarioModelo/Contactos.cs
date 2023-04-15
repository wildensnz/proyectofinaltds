using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Contactos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Direccion { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Extension { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = "";
        public bool Activo { get; set; } = true;
        public int? EmpresaId { get; set; }
        public int UsuarioId { get; set; }

        public virtual IEnumerable<Compras> Compras { get; set; } = new List<Compras>();

        [ForeignKey("EmpresaId")]
        public virtual Empresas? Empresas { get; set; } = new Empresas();

        [ForeignKey("UsuarioId")]
        public virtual Usuarios Usuarios { get; set; } = new Usuarios();

        public virtual IEnumerable<Ventas> Ventas { get; set; } = new List<Ventas>();
    }
}
