using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioViewModel
{
    public class RolesPermisosViewModel
    {
        public string Modulo { get; set; } = "";
        public bool Ver { get; set; }
        public bool Agregar { get; set; }
        public bool Modificar { get; set; }
        public bool Eliminar { get; set; }
    }
}
