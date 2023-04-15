using InventarioHerramienta;
using InventarioHerramienta.Interfaces;
using InventarioModelo;
using InventarioViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SistemaInventario.DataContext;
using SistemaInventario.Filters;
using SistemaInventario.Helpers;

namespace SistemaInventario.Controllers
{
    public class UsuariosController : BaseController

    {
        protected IOptions<HashingOptions> options;
        public UsuariosController(IMailService _mailSender, InventarioDbContext _dbContext, IOptions<HashingOptions> _options) : base(_mailSender, _dbContext)
        {
            this.options = _options;
        }
        [Authorize("Accesos", "Ver")]
        public IActionResult Index()
        {
            tienePermiso("Accesos", new List<RolPermisos>());

            var roles = dbContext.Roles.Where(c => c.Activo == true).OrderBy(c =>
           c.Rol).ToList();
            ViewBag.Roles = roles;
            return View();
        }

        [Authorize("Accesos", "Ver")]
        public async Task<ActionResult> Lista(string texto, int page = 1, int reg = 10)
        {
            tienePermiso("Accesos", new List<RolPermisos>());

            int totalItem = 0;
            List<Usuarios> usuarios = new List<Usuarios>();
            if (texto == null || texto.Trim().Length == 0)
            {
                totalItem = await dbContext.Usuarios.Where(c => c.Estatus == true).CountAsync();
                Pager pager = new Pager(totalItem, page, reg);
                ViewBag.Pager = pager;
                if (totalItem > 0)
                    usuarios = await dbContext.Usuarios.Where(c => c.Estatus == true).Include(c => c.Roles).OrderByDescending(c => c.Nombre).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }
            else
            {
                totalItem = await dbContext.Usuarios.Where(c => c.Estatus == true && (c.Nombre.Contains(texto) ||
                c.Email.Contains(texto))).CountAsync();
                Pager pager = new Pager(totalItem, page, reg);
                ViewBag.Pager = pager;
                if (totalItem > 0)
                    usuarios = await dbContext.Usuarios.Where(c => c.Estatus == true && (c.Nombre.Contains(texto) ||
                c.Email.Contains(texto))).Include(c => c.Roles).OrderByDescending(c => c.Nombre).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }

            return PartialView(usuarios);
        }


        [HttpPost]
        [Authorize("Accesos", "AgregarModificar")]
        public async Task<ActionResult<Response>> Save(Usuarios e)
        {
            try
            {
                if (HttpContext.Response.StatusCode == 401)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No tienes permisos para realizar esta acción."
                    };
                string pass = "";
                if (e.Password != null && e.Password.Length > 0)
                {
                    PasswordHasher passwordHasher = new PasswordHasher(options);
                    pass = passwordHasher.Hash(e.Password);

                }
                int usuarioId = AutenticacionHelper.GetUsuario(HttpContext);

                if (e.Id > 0)
                {
                    if (!AutenticacionHelper.tienePermiso("Accesos", "Modificar", GetPermisos()))
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No tienes permisos para modificar la información."
                        };
                    var mdb = await dbContext.Usuarios.Where(c => c.Id == e.Id).FirstOrDefaultAsync();
                    if (mdb != null)
                    {

                        mdb.Nombre = e.Nombre;
                        mdb.Estatus = true;
                        mdb.Email = e.Email;
                        mdb.RolId = e.RolId;
                        mdb.Password = pass.Length > 0 ? pass : mdb.Password;
                        mdb.UsuarioPadreId = usuarioId;
                        mdb.Roles = await dbContext.Roles.Where(c => c.Id == e.RolId).FirstOrDefaultAsync() ?? new Roles();
                        dbContext.Update(mdb);
                    }
                    else
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No se encuentra el usuario que quieres modificar."
                        };
                    }
                }
                else
                {
                    if (!AutenticacionHelper.tienePermiso("Accesos", "Agregar", GetPermisos()))
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No tienes permisos para agregar registros."
                        };
                    e.Fecha = DateTime.Now;
                    e.Password = pass.Length > 0 ? pass : e.Password == null || e.Password.Length == 0 ? "" : e.Password;
                    e.Estatus = true;
                    e.UsuarioPadreId = usuarioId;
                    e.Roles = await dbContext.Roles.Where(c => c.Id == e.RolId).FirstOrDefaultAsync() ?? new Roles();
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
        [Authorize("Accesos", "Ver")]
        public async Task<ActionResult<Response>> Get(int Id)
        {
            try
            {
                if (HttpContext.Response.StatusCode == 401)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No tienes permisos para realizar esta acción."
                    };
                var respuesta = new Response();
                var usuario = await dbContext.Usuarios.Where(c => c.Id == Id).Include(c => c.Roles).FirstOrDefaultAsync();
                if (usuario == null)
                {
                    respuesta.IsSuccess = false;
                    respuesta.Message = "No encontre el usuario que quieres modificar";
                }
                else
                {
                    respuesta.IsSuccess = true;
                    respuesta.Message = "Información correcta";
                    respuesta.Result = usuario;
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
        [Authorize("Accesos", "Eliminar")]
        public async Task<ActionResult<Response>> Delete(string Id)
        {
            if (HttpContext.Response.StatusCode == 401)
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
                var permisos = await dbContext.Usuarios.Where(c => ids.Contains(c.Id)).ToListAsync();
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
