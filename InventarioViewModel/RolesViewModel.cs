using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioViewModel
{
    public class RolesViewModel
    {
        public int Id { get; set; } 
        public string Rol { get; set; }

        public List<RolesPermisosViewModel> permisosViewModels { get; set; }
    }
}
