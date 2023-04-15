using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Tokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Token { get; set; } = "";
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuarios Usuarios { get; set; } = new Usuarios();

    }
}
