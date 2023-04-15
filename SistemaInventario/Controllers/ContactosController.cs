using InventarioHerramienta.Interfaces;
using InventarioHerramienta;
using InventarioModelo;
using InventarioViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataContext;
using SistemaInventario.Helpers;
using SistemaInventario.Filters;

namespace SistemaInventario.Controllers
{
    public class ContactosController : BaseController
    {
        public ContactosController(IMailService _mailSender, InventarioDbContext _dbContext) : base(_mailSender, _dbContext)
        {
        }

        [Authorize("Contactos", "Ver")]
        public async Task<IActionResult> Index()
        {
            tienePermiso("Contactos", new List<RolPermisos>());
            ViewBag.Empresas = await dbContext.Empresas.Where(c => c.Activo == true).ToListAsync();
            // var roles = dbContext.Roles.Where(c => c.Activo == true).OrderBy(c =>
            //c.Rol).ToList();
            // ViewBag.Roles = roles;
            return View();
        }

        [Authorize("Contactos", "Ver")]
        public async Task<ActionResult> Lista(string texto, int page = 1, int reg = 10)
        {
            tienePermiso("Contactos", new List<RolPermisos>());

            int totalItem = 0;
            List<Contactos> contactos = new List<Contactos>();
            if (texto == null || texto.Trim().Length == 0)
            {
                totalItem = await dbContext.Contactos.Where(c => c.Activo == true).CountAsync();
                Pager pager = new Pager(totalItem, page, reg);
                ViewBag.Pager = pager;
                if (totalItem > 0)
                    contactos = await dbContext.Contactos.Where(c => c.Activo == true).Include(c => c.Empresas).OrderByDescending(c => c.Nombre).
                        Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }
            else
            {
                totalItem = await dbContext.Contactos.Where(c => c.Activo == true && (c.Nombre.Contains(texto) ||
                c.Email.Contains(texto) || c.Telefono.Contains(texto) || c.Tipo.Contains(texto))).CountAsync();
                Pager pager = new Pager(totalItem, page, reg);
                ViewBag.Pager = pager;
                if (totalItem > 0)
                    contactos = await dbContext.Contactos.Where(c => c.Activo == true && (c.Nombre.Contains(texto) ||
                c.Email.Contains(texto) || c.Telefono.Contains(texto) || c.Tipo.Contains(texto))).Include(c => c.Empresas).
                OrderByDescending(c => c.Nombre).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }

            return PartialView(contactos);
        }


        [HttpPost]
        [Authorize("Contactos", "AgregarModificar")]
        public async Task<ActionResult<Response>> Save(Contactos e)
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
                var empresa = e.EmpresaId != null ? dbContext.Empresas.Where(c => c.Id == e.EmpresaId).FirstOrDefault() : new Empresas();
                var usu = await dbContext.Usuarios.Where(c => c.Id == usuarioId).FirstOrDefaultAsync() ?? new Usuarios();
                if (e.Id > 0)
                {
                    if (!AutenticacionHelper.tienePermiso("Contactos", "Modificar", GetPermisos()))
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No tienes permisos para modificar la información."
                        };
                    var mdb = await dbContext.Contactos.Where(c => c.Id == e.Id).FirstOrDefaultAsync();
                    if (mdb != null)
                    {

                        mdb.Nombre = e.Nombre;
                        mdb.Direccion = e.Direccion;
                        mdb.Telefono = e.Telefono;
                        mdb.Extension = e.Extension;
                        mdb.Email = e.Email;
                        mdb.Fecha = DateTime.Now;
                        mdb.Tipo = e.Tipo;
                        mdb.Activo = true;
                        mdb.EmpresaId = e.EmpresaId;
                        mdb.UsuarioId = usuarioId;
                        mdb.Empresas = empresa;
                        mdb.Usuarios = usu;
                        dbContext.Update(mdb);
                    }
                    else
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No se encuentra el contacto que quieres modificar."
                        };
                    }
                }
                else
                {
                    if (!AutenticacionHelper.tienePermiso("Contactos", "Agregar", GetPermisos()))
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No tienes permisos para agregar registros."
                        };
                    e.Fecha = DateTime.Now;
                    e.Activo = true;
                    e.UsuarioId = usuarioId;
                    e.Empresas = empresa;
                    e.Usuarios = usu;

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
        [Authorize("Contactos", "Ver")]
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
                var contacto = await dbContext.Contactos.Where(c => c.Id == Id).Include(c => c.Empresas).FirstOrDefaultAsync();
                if (contacto == null)
                {
                    respuesta.IsSuccess = false;
                    respuesta.Message = "No encontre el contacto que quieres modificar";
                }
                else
                {
                    respuesta.IsSuccess = true;
                    respuesta.Message = "Información correcta";
                    respuesta.Result = contacto;
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
        [Authorize("Contactos", "Eliminar")]
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
                var permisos = await dbContext.Contactos.Where(c => ids.Contains(c.Id)).ToListAsync();
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
