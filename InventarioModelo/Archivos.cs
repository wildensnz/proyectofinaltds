using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Archivos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Archivo { get; set; } = "";
        public string Tipo { get; set; } = "";
        public bool Activo { get; set; } = true;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
