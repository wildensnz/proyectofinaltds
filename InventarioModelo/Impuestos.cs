using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Impuestos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = 0;
        public string Nombre { get; set; } = "";
        public decimal Valor { get; set; } = 0;
        public bool EsPorcentaje { get; set; } = false;
        public bool AplicarDefault { get; set; } = false;
        public bool Activo { get; set; } = true;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
