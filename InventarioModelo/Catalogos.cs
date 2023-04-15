using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Catalogos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Texto { get; set; } = "";
        public string Valor { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Catalogo { get; set; } = "";
        public int Orden { get; set; } = 1;
        public bool Activo { get; set; } = true;
    }
}
