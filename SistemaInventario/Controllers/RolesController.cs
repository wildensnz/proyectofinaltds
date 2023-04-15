using InventarioHerramienta;
using InventarioHerramienta.Interfaces;
using InventarioModelo;
using InventarioViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataContext;

namespace SistemaInventario.Controllers
{
    public class RolesController : BaseController
    {
        public RolesController(IMailService _mailSender, InventarioDbContext _dbContext) : base(_mailSender, _dbContext)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Lista(string texto, int page = 1)
        {
            int totalItem = 0;
            List<Roles> roles = new List<Roles>();
            if (texto == null || texto.Trim().Length == 0)
            {
                totalItem = await dbContext.Roles.Where(c => c.Activo == true).CountAsync();
                Pager pager = new Pager(totalItem, page, 10);
                ViewBag.Pager = pager;
                roles = await dbContext.Roles.Where(c => c.Activo == true).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }
            else
            {
                totalItem = await dbContext.Roles.Where(c => c.Activo == true && c.Rol.Contains(texto)).CountAsync();
                Pager pager = new Pager(totalItem, page, 10);
                ViewBag.Pager = pager;
                roles = await dbContext.Roles.Where(c => c.Activo == true && c.Rol.Contains(texto)).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }
            foreach (var role in roles)
            {
                role.TotalUsuarios = await dbContext.Usuarios.Where(c => c.RolId == role.Id && c.Estatus == true).CountAsync();
            }

            return PartialView(roles);
        }






        [HttpPost]
        public async Task<ActionResult<Response>> Permisos(int Id, string Permisos, string rol)
        {
            try
            {
                string[] permisos = Permisos.Split("|");
                List<RolPermisos> rolPermisos = new List<RolPermisos>();
                rolPermisos = await dbContext.RolPermisos.Where(c => c.RolId == Id && c.Activo == true).ToListAsync();
                var roles = await dbContext.Roles.Where(c => c.Id == Id).FirstOrDefaultAsync();
                if (roles == null && Id > 0)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No se encuentra el rol que quieres modificar"
                    };
                else
                {
                    if (roles == null)
                    {
                        roles = new Roles
                        {
                            Activo = true,
                            Fecha = DateTime.Now,
                            Rol = rol,
                            TotalUsuarios = 0
                        };
                        dbContext.Add(roles);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        roles.Rol = rol;
                        await dbContext.SaveChangesAsync();
                    }

                }
                int count = rolPermisos.Count;
                if (permisos.Length > 6)
                {
                    foreach (var item in permisos)
                    {
                        string[] valores = item.Split(",");
                        if (valores.Length > 3)
                        {
                            if (count == 0)
                            {

                                dbContext.Add(new RolPermisos
                                {
                                    Activo = true,
                                    Modulo = valores[0],
                                    Ver = valores[1] == "false" || valores[1] == "true" ? bool.Parse(valores[1]) : false,
                                    Agregar = valores[2] == "false" || valores[2] == "true" ? bool.Parse(valores[2]) : false,
                                    Modificar = valores[3] == "false" || valores[3] == "true" ? bool.Parse(valores[3]) : false,
                                    Eliminar = valores[4] == "false" || valores[4] == "true" ? bool.Parse(valores[4]) : false,
                                    RolId = Id,
                                    Fecha = DateTime.Now,
                                    Roles = roles
                                });
                            }
                            else
                            {
                                foreach (var itemdb in rolPermisos.Where(c => c.Modulo == valores[0]))
                                {
                                    itemdb.Modulo = valores[0];
                                    itemdb.Ver = valores[1] == "false" || valores[1] == "true" ? bool.Parse(valores[1]) : false;
                                    itemdb.Agregar = valores[2] == "false" || valores[2] == "true" ? bool.Parse(valores[2]) : false;
                                    itemdb.Modificar = valores[3] == "false" || valores[3] == "true" ? bool.Parse(valores[3]) : false;
                                    itemdb.Eliminar = valores[4] == "false" || valores[4] == "true" ? bool.Parse(valores[4]) : false;
                                }
                            }

                        }
                    }
                    await dbContext.SaveChangesAsync();
                }

                return new Response
                {
                    IsSuccess = true,
                    Message = "Se guardo la información correctamente"
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
        public async Task<ActionResult<Response>> Permisos(int Id)
        {
            try
            {
                var respuesta = new Response();
                var permiso = await dbContext.Roles.Where(c => c.Id == Id).Include(c => c.RolPermisos).FirstOrDefaultAsync();
                if (permiso == null)
                {
                    respuesta.IsSuccess = false;
                    respuesta.Message = "No encontre el rol que quieres modificar";
                }
                else
                {
                    var rolViewModel = new RolesViewModel
                    {
                        Id = permiso.Id,
                        Rol = permiso.Rol,
                        permisosViewModels = permiso.RolPermisos.Select(c => new RolesPermisosViewModel
                        {
                            Modulo = c.Modulo,
                            Agregar = c.Agregar,
                            Eliminar = c.Eliminar,
                            Modificar = c.Modificar,
                            Ver = c.Ver
                        }).ToList()
                    };
                    respuesta.IsSuccess = true;
                    respuesta.Message = "Se guardo la información correctamente";
                    respuesta.Result = rolViewModel;
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
                var permisos = await dbContext.Roles.Where(c => ids.Contains(c.Id)).ToListAsync();
                foreach (var item in permisos)
                {
                    item.Activo = false;
                }
                dbContext.SaveChanges();
                respuesta.IsSuccess = true;
                respuesta.Message = "Se elimino el rol correctamente";
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
