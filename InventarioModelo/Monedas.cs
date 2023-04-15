using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Monedas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Moneda { get; set; } = "";
        public string Simbolo { get; set; } = "$";
        public int Decimales { get; set; } = 2;
        public string SeparadorMiles { get; set; } = ",";
        public string SeparadorDecimales { get; set; } = ".";
        public string Codigo { get; set; } = "DOP";
        public string Pais { get; set; } = "República Dominicana";
        public bool Principal { get; set; }
        public bool Activo { get; set; }
    }
}
