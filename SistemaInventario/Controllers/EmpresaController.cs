using InventarioHerramienta;
using InventarioHerramienta.Interfaces;
using InventarioModelo;
using InventarioViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataContext;
using SistemaInventario.Helpers;
using SistemaInventario.Filters;

namespace SistemaInventario.Controllers
{

    public class EmpresaController : BaseController
    {
        public EmpresaController(IMailService _mailSender, InventarioDbContext _dbContext) : base(_mailSender, _dbContext)
        {

        }
        [Authorize("Configuración", "Ver")]
        public IActionResult Index()
        {
            var tEmpresa = dbContext.Catalogos.Where(c => c.Activo == true && c.Catalogo == "TipoEmpresa").OrderBy(c => c.Orden).ThenBy(c => c.Texto).ToList();
            var tPaises = dbContext.Catalogos.Where(c=> c.Activo == true && c.Catalogo == "Paises").OrderBy(c=> c.Orden).ThenBy(c=> c.Texto).ToList();
            var monedas = dbContext.Monedas.Where(c => c.Activo == true).OrderByDescending(c => c.Principal).ThenBy(c =>c.Moneda).ToList();
            ViewBag.Monedas = monedas;
            ViewBag.tEmpresa = tEmpresa;
            ViewBag.tPaises = tPaises;
            return View();
        }

        [Authorize("Configuración", "Ver")]
        public async Task<ActionResult> Lista(string texto, int page = 1, int reg = 10)
        {
            tienePermiso("Configuración", new List<RolPermisos>());
            int totalItem = 0;
            List<Empresas> empresas = new List<Empresas>();
            if (texto == null || texto.Trim().Length == 0)
            {
                totalItem = await dbContext.Empresas.Where(c => c.Activo == true).CountAsync();
                Pager pager = new Pager(totalItem, page, reg);
                ViewBag.Pager = pager;
                if (totalItem > 0)
                    empresas = await dbContext.Empresas.Where(c => c.Activo == true).Include(c => c.Logo).OrderByDescending(c => c.Nombre).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }
            else
            {
                totalItem = await dbContext.Empresas.Where(c => c.Activo == true && (c.Nombre.Contains(texto) ||
                c.Ciudad.Contains(texto) || c.CodigoPostal.Contains(texto))).CountAsync();
                Pager pager = new Pager(totalItem, page, reg);
                ViewBag.Pager = pager;
                if (totalItem > 0)
                    empresas = await dbContext.Empresas.Where(c => c.Activo == true && (c.Nombre.Contains(texto) ||
                c.Ciudad.Contains(texto) || c.CodigoPostal.Contains(texto))).Include(c => c.Logo).OrderByDescending(c => c.Nombre).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }

            return PartialView(empresas);
        }

        [HttpPost]
        [Authorize("Configuración", "AgregarModificar")]
        public async Task<ActionResult<Response>> Save(Empresas e, IFormFile LogoId)
        {
            try
            {
                if (HttpContext.Response.StatusCode == 401 || HttpContext.Response.StatusCode == 403)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No tienes permisos para realizar esta acción."
                    };

                int usuarioId = AutenticacionHelper.GetUsuario(HttpContext);
                if (LogoId != null && LogoId.Length > 0)
                {

                    string nammeFile = "Empresa_" + usuarioId + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day +
                        "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + Path.GetExtension(LogoId.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images/upload", nammeFile);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await LogoId.CopyToAsync(stream);
                        var logo = new Archivos
                        {
                            Activo = true,
                            Archivo = nammeFile,
                            Fecha = DateTime.Now,
                            Tipo = "Logo"
                        };
                        dbContext.Archivos.Add(logo);
                        await dbContext.SaveChangesAsync();
                        e.Logo = logo;
                        e.LogoId = logo.Id;
                    }
                }

                if (e.Id > 0)
                {
                    var mdb = await dbContext.Empresas.Where(c => c.Id == e.Id).FirstOrDefaultAsync();
                    if (mdb != null)
                    {
                        mdb.Descripcion = e.Descripcion;
                        mdb.Activo = true;
                        mdb.Contactos = e.Contactos;
                        mdb.Calle = e.Calle;
                        mdb.CorreoElectronico = e.CorreoElectronico;
                        mdb.CodigoPostal = e.CodigoPostal;
                        mdb.Nombre = e.Nombre;
                        mdb.Ciudad = e.Ciudad;
                        mdb.Telefono = e.Telefono;
                        mdb.MonedaId = e.MonedaId;
                        if (e.LogoId != null && e.LogoId > 0)
                        {
                            mdb.LogoId = e.LogoId;
                            mdb.Logo = e.Logo;
                        }
                        mdb.Estado = e.Estado;
                        mdb.PaisId = e.PaisId;
                        mdb.TipoEmpresaId = e.TipoEmpresaId;
                        mdb.Pais = await dbContext.Catalogos.Where(c => c.Id == e.PaisId).FirstOrDefaultAsync() ?? new Catalogos();
                        mdb.TipoEmpresa = await dbContext.Catalogos.Where(c => c.Id == e.TipoEmpresaId).FirstOrDefaultAsync() ?? new Catalogos();
                        dbContext.Update(mdb);
                    }
                    else
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No se encuentra la empresa que quieres modificar."
                        };
                    }
                }
                else
                {
                    e.Fecha = DateTime.Now;
                    e.Activo = true;
                    e.Pais = await dbContext.Catalogos.Where(c => c.Id == e.PaisId).FirstOrDefaultAsync() ?? new Catalogos();
                    e.TipoEmpresa = await dbContext.Catalogos.Where(c => c.Id == e.TipoEmpresaId).FirstOrDefaultAsync() ?? new Catalogos();
                    e.Usuarios = await dbContext.Usuarios.Where(c => c.Id == usuarioId).FirstOrDefaultAsync() ?? new Usuarios();
                    dbContext.Add(e);
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
        [Authorize("Configuración", "Ver")]
        public async Task<ActionResult<Response>> Get(int Id)
        {
            try
            {
                if (HttpContext.Response.StatusCode == 401 || HttpContext.Response.StatusCode == 403)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No tienes permisos para realizar esta acción."
                    };
                var respuesta = new Response();
                var empresa = await dbContext.Empresas.Where(c => c.Id == Id).Include(c => c.Logo).FirstOrDefaultAsync();
                if (empresa == null)
                {
                    respuesta.IsSuccess = false;
                    respuesta.Message = "No encontre la empresa que quieres modificar";
                }
                else
                {
                    respuesta.IsSuccess = true;
                    respuesta.Message = "Información correcta";
                    respuesta.Result = empresa;
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
        [Authorize("Configuración", "Eliminar")]
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
                var permisos = await dbContext.Empresas.Where(c => ids.Contains(c.Id)).ToListAsync();
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
