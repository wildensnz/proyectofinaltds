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
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Extension { get; set; }
        public string Correo { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public string Estatus { get; set; }
        public int Empresa_ID { get; set; }
        public int Usuario_ID { get; set; }

        public Contactos()
        {
            this.Nombre = "";
            this.Direccion = "";
            this.Telefono = "";
            this.Extension = "";
            this.Correo = "";
            this.Fecha = DateTime.Now;
            this.Tipo = "";
            this.Estatus = "";
        }
    }
}
