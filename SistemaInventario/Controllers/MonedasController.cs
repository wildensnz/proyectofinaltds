using InventarioHerramienta;
using InventarioHerramienta.Interfaces;
using InventarioModelo;
using InventarioViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataContext;

namespace SistemaInventario.Controllers
{
    public class MonedasController : BaseController
    {
        public MonedasController(IMailService _mailSender, InventarioDbContext _dbContext) : base(_mailSender, _dbContext)
        {

        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Lista(string texto, int page = 1, int reg = 10)
        {
            int totalItem = 0;
            List<Monedas> monedas = new List<Monedas>();
            if (texto == null || texto.Trim().Length == 0)
            {
                totalItem = await dbContext.Monedas.Where(c => c.Activo == true).CountAsync();
                Pager pager = new Pager(totalItem, page, reg);
                ViewBag.Pager = pager;
                monedas = await dbContext.Monedas.Where(c => c.Activo == true).OrderByDescending(c => c.Principal).ThenBy(c => c.Moneda).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }
            else
            {
                totalItem = await dbContext.Monedas.Where(c => c.Activo == true && (c.Moneda.Contains(texto) ||
                c.Pais.Contains(texto) || c.Codigo.Contains(texto))).CountAsync();
                Pager pager = new Pager(totalItem, page, reg);
                ViewBag.Pager = pager;
                monedas = await dbContext.Monedas.Where(c => c.Activo == true && (c.Moneda.Contains(texto) ||
                c.Pais.Contains(texto) || c.Codigo.Contains(texto))).OrderByDescending(c => c.Principal).ThenBy(c => c.Moneda).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }

            return PartialView(monedas);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> Save(Monedas monedas)
        {
            try
            {
                if (monedas.Id > 0)
                {
                    var mdb = await dbContext.Monedas.Where(c => c.Id == monedas.Id).FirstOrDefaultAsync();
                    if (mdb != null)
                    {
                        mdb.SeparadorMiles = monedas.SeparadorMiles;
                        mdb.SeparadorDecimales = monedas.SeparadorDecimales;
                        mdb.Decimales = monedas.Decimales;
                        mdb.Moneda = monedas.Moneda;
                        mdb.Activo = true;
                        mdb.Principal = monedas.Principal;
                        mdb.Codigo = monedas.Codigo;
                        mdb.Pais = monedas.Pais;
                        mdb.Simbolo = monedas.Simbolo;
                        dbContext.Update(mdb);
                    }
                    else
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No se encuentra la moneda que quieres modificar."
                        };
                    }
                }
                else
                {
                    monedas.Activo = true;
                    dbContext.Add(monedas);
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
                var moneda = await dbContext.Monedas.Where(c => c.Id == Id).FirstOrDefaultAsync();
                if (moneda == null)
                {
                    respuesta.IsSuccess = false;
                    respuesta.Message = "No encontre la moneda que quieres modificar";
                }
                else
                {
                    respuesta.IsSuccess = true;
                    respuesta.Message = "Información correcta";
                    respuesta.Result = moneda;
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
                var permisos = await dbContext.Monedas.Where(c => ids.Contains(c.Id)).ToListAsync();
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
