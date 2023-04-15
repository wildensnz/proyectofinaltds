using InventarioHerramienta.Interfaces;
using InventarioHerramienta;
using InventarioModelo;
using InventarioViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataContext;
using SistemaInventario.Filters;

namespace SistemaInventario.Controllers
{
    public class ComprasController : BaseController
    {
        public ComprasController(IMailService _mailSender, InventarioDbContext _dbContext) : base(_mailSender, _dbContext)
        {
        }

        [Authorize("Compras", "Ver")]
        public IActionResult Index()
        {
            tienePermiso("Compras", new List<RolPermisos>());
            return View();
        }

        [Authorize("Compras", "Ver")]
        public async Task<ActionResult> Lista(string texto, int page = 1, int reg = 10)
        {
            tienePermiso("Compras", new List<RolPermisos>());

            int totalItem = 0;
            List<Compras> compras = new List<Compras>();
            if (texto == null || texto.Trim().Length == 0)
            {
                totalItem = await dbContext.Compras.Where(c => c.Estatus == true).CountAsync();
                crerPaginacion(totalItem, page, reg);
                if (totalItem > 0)
                    compras = await dbContext.Compras.Where(c => c.Estatus == true).
                        Include(c => c.Contactos).Include(c => c.Usuarios).OrderByDescending(c => c.Fecha).
                        Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }
            else
            {
                var provIds = await dbContext.Contactos.Where(c => c.Nombre.Contains(texto) ||
                c.Email.Contains(texto)).Select(c => c.Id).ToListAsync();
                if (provIds.Count() > 0)
                    totalItem = await dbContext.Compras.Where(c => c.Estatus == true &&
                (c.CodigoFactura.Contains(texto) || provIds.Contains(c.ProveedorId))).CountAsync();
                else
                    totalItem = await dbContext.Compras.Where(c => c.Estatus == true &&
                    c.CodigoFactura.Contains(texto)).CountAsync();

                crerPaginacion(totalItem, page, reg);
                if (totalItem > 0 && provIds.Count() > 0)
                    compras = await dbContext.Compras.Where(c => c.Estatus == true &&
                    (c.CodigoFactura.Contains(texto) || provIds.Contains(c.ProveedorId))).Include(c => c.Contactos).
                    Include(c => c.Usuarios).OrderByDescending(c => c.Fecha).
                    Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
                else if (totalItem > 0)
                    compras = await dbContext.Compras.Where(c => c.Estatus == true &&
                    c.CodigoFactura.Contains(texto)).Include(c => c.Contactos).
                    Include(c => c.Usuarios).OrderByDescending(c => c.Fecha).
                    Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }
            return PartialView(compras);
        }


        [HttpGet]
        [Authorize("Compras", "Eliminar")]
        public async Task<ActionResult<Response>> Delete(string Id)
        {
            if (HttpContext.Response.StatusCode == 401 || HttpContext.Response.StatusCode == 403)
                return new Response
                {
                    IsSuccess = false,
                    Message = "No tienes permisos para realizar esta acción."
                };
            List<int> ids = new List<int>();
            foreach (var item in Id.Split("|"))
            {
                int val = 0;
                int.TryParse(item, out val);
                if (val > 0)
                    ids.Add(val);
            }
            try
            {
                var respuesta = new Response();
                var permisos = await dbContext.Compras.Where(c => ids.Contains(c.Id)).ToListAsync();
                foreach (var item in permisos)
                {
                    item.Estatus = false;
                }
                dbContext.SaveChanges();
                respuesta.IsSuccess = true;
                respuesta.Message = "Se elimino la información correctamente";
                return respuesta;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.ToString()
                };
            }
        }
    }
}
