using InventarioHerramienta;
using InventarioHerramienta.Interfaces;
using InventarioModelo;
using InventarioViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataContext;

namespace SistemaInventario.Controllers
{
    [Authorize]
    public class ImpuestosController : BaseController
    {
        public ImpuestosController(IMailService _mailSender, InventarioDbContext _dbContext) : base(_mailSender, _dbContext)
        {

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Lista(string texto, int page = 1, int reg = 10)
        {
            int totalItem = 0;
            List<Impuestos> imp = new List<Impuestos>();
            if (texto == null || texto.Trim().Length == 0)
            {
                totalItem = await dbContext.Impuestos.Where(c => c.Activo == true).CountAsync();
                Pager pager = new Pager(totalItem, page, reg);
                ViewBag.Pager = pager;
                imp = await dbContext.Impuestos.Where(c => c.Activo == true).OrderByDescending(c => c.AplicarDefault).ThenBy(c => c.Nombre).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }
            else
            {
                totalItem = await dbContext.Impuestos.Where(c => c.Activo == true && c.Nombre.Contains(texto)).CountAsync();
                Pager pager = new Pager(totalItem, page, reg);
                ViewBag.Pager = pager;
                imp = await dbContext.Impuestos.Where(c => c.Activo == true && c.Nombre.Contains(texto)).OrderByDescending(c => c.AplicarDefault).ThenBy(c => c.Nombre).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }

            return PartialView(imp);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> Save(Impuestos imp)
        {
            try
            {
                if (imp.Id > 0)
                {
                    var idb = await dbContext.Impuestos.Where(c => c.Id == imp.Id).FirstOrDefaultAsync();
                    if (idb != null)
                    {
                        idb.Nombre = imp.Nombre;
                        idb.EsPorcentaje = imp.EsPorcentaje;
                        idb.Activo = true;
                        idb.AplicarDefault = imp.AplicarDefault;
                        idb.Valor = imp.Valor;
                        dbContext.Update(idb);
                    }
                    else
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No se encuentra el impuesto que quieres modificar."
                        };
                    }
                }
                else
                {
                    imp.Activo = true;
                    dbContext.Add(imp);
                }
                await dbContext.SaveChangesAsync();
                return new Response
                {
                    IsSuccess = true,
                    Message = "Se guardo la información correctamente."
                };
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


        [HttpGet]
        public async Task<ActionResult<Response>> Get(int Id)
        {
            try
            {
                var respuesta = new Response();
                var imp = await dbContext.Impuestos.Where(c => c.Id == Id).FirstOrDefaultAsync();
                if (imp == null)
                {
                    respuesta.IsSuccess = false;
                    respuesta.Message = "No encontre el impuesto que quieres modificar";
                }
                else
                {
                    respuesta.IsSuccess = true;
                    respuesta.Message = "Información correcta";
                    respuesta.Result = imp;
                }
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

        [HttpGet]
        public async Task<ActionResult<Response>> Delete(string Id)
        {
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
                var permisos = await dbContext.Impuestos.Where(c => ids.Contains(c.Id)).ToListAsync();
                foreach (var item in permisos)
                {
                    item.Activo = false;
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
