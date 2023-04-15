using InventarioHerramienta;
using InventarioHerramienta.Interfaces;
using InventarioModelo;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DataContext;
using SistemaInventario.Helpers;

namespace SistemaInventario.Controllers
{
    public class BaseController : Controller
    {

        protected readonly InventarioDbContext dbContext;
        protected readonly IMailService mailSender;
        public Pager pager;

        public BaseController(IMailService _mailSender, InventarioDbContext _dbContext)
        {
            dbContext = _dbContext;  
            mailSender = _mailSender;
        }

        public List<RolPermisos> GetPermisos()
        {
            int rolId = AutenticacionHelper.GetValueClaim(HttpContext, "RolId");
            var permisos = dbContext.RolPermisos.Where(c => c.RolId == rolId && c.Activo == true && (
            c.Eliminar == true || c.Agregar == true || c.Ver == true || c.Modificar == true)).ToList();
            return permisos;
        }

        public void tienePermiso(string modulo, List<RolPermisos> rolesMemoria)
        {
            var permisos = rolesMemoria.Count() > 0 ? rolesMemoria : GetPermisos();
            ViewBag.Permisos = permisos;
            ViewBag.tPermiso = permisos.Where(c => c.Modulo == modulo && (c.Agregar == true ||
            c.Eliminar == true || c.Modificar == true || c.Ver == true)).FirstOrDefault();

        }

        public void crerPaginacion(int totalItem = 0, int page = 1, int reg = 10)
        {
            Pager _pager = new Pager(totalItem, page, reg);
            pager = _pager;
            ViewBag.Pager = _pager;
        }
    }
}
