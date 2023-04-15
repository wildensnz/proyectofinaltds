using InventarioHerramienta.Interfaces;
using InventarioModelo;
using InventarioViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataContext;
using SistemaInventario.Filters;
using SistemaInventario.Helpers;

namespace SistemaInventario.Controllers
{
    public class ProductosController : BaseController
    {
        public ProductosController(IMailService _mailSender, InventarioDbContext _dbContext) : base(_mailSender, _dbContext)
        {
        }

        [Authorize("Productos", "Ver")]
        public async Task<IActionResult> Index()
        {
            tienePermiso("Productos", new List<RolPermisos>());
            ViewBag.Empresas = await dbContext.Empresas.Where(c => c.Activo == true).ToListAsync();
            ViewBag.Ipuestos = await dbContext.Impuestos.Where(c => c.Activo == true).OrderBy(c => c.AplicarDefault).ToListAsync();
            return View();
        }


        [Authorize("Productos", "Ver")]
        public async Task<ActionResult> Lista(string texto, int page = 1, int reg = 10)
        {
            tienePermiso("Productos", new List<RolPermisos>());

            int totalItem = 0;
            List<Productos> productos = new List<Productos>();
            if (texto == null || texto.Trim().Length == 0)
            {
                totalItem = await dbContext.Productos.Where(c => c.Estatus == true).CountAsync();
                crerPaginacion(totalItem, page, reg);
                if (totalItem > 0)
                    productos = await dbContext.Productos.Where(c => c.Estatus == true).Include(c => c.Empresas).Include(c => c.Archivos).OrderByDescending(c => c.Nombre).
                        Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }
            else
            {
                totalItem = await dbContext.Productos.Where(c => c.Estatus == true && (c.Nombre.Contains(texto) ||
                c.Codigo.Contains(texto) || c.Descripcion.Contains(texto) || c.Modelo.Contains(texto))).CountAsync();
                crerPaginacion(totalItem, page, reg);
                if (totalItem > 0)
                    productos = await dbContext.Productos.Where(c => c.Estatus == true && (c.Nombre.Contains(texto) ||
                c.Codigo.Contains(texto) || c.Descripcion.Contains(texto) || c.Modelo.Contains(texto))).Include(c => c.Empresas).Include(c => c.Archivos).
                OrderByDescending(c => c.Nombre).Skip(pager.StartIndex).Take(pager.PageSize).ToListAsync();
            }

            return PartialView(productos);
        }


        [HttpPost]
        [Authorize("Productos", "AgregarModificar")]
        public async Task<ActionResult<Response>> Save(Productos e, IFormFile producto)
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
                var empresa = await dbContext.Empresas.Where(c => c.Id == e.EmpresaId).FirstOrDefaultAsync();
                var usu = await dbContext.Usuarios.Where(c => c.Id == usuarioId).FirstOrDefaultAsync() ?? new Usuarios();

                Archivos archivo = null;

                if (producto != null && producto.Length > 0)
                {
                    string nammeFile = "Producto_" + usuarioId + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day +
                       "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + Path.GetExtension(producto.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images/upload", nammeFile);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await producto.CopyToAsync(stream);
                        archivo = new Archivos
                        {
                            Activo = true,
                            Archivo = nammeFile,
                            Fecha = DateTime.Now,
                            Tipo = "Producto"
                        };
                        dbContext.Archivos.Add(archivo);
                        await dbContext.SaveChangesAsync();
                    }
                }


                if (e.Id > 0)
                {
                    if (!AutenticacionHelper.tienePermiso("Productos", "Modificar", GetPermisos()))
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No tienes permisos para modificar la información."
                        };
                    var mdb = await dbContext.Productos.Where(c => c.Id == e.Id).FirstOrDefaultAsync();
                    if (mdb != null)
                    {

                        mdb.Codigo = e.Codigo;
                        mdb.Nombre = e.Nombre;
                        mdb.Descripcion = e.Descripcion;
                        mdb.Modelo = e.Modelo;
                        mdb.Presentacion = e.Presentacion;
                        mdb.Marca = e.Marca;
                        mdb.Costo = e.Costo;
                        mdb.PrecioVenta = e.PrecioVenta;
                        mdb.Stock = e.Stock;
                        mdb.Estatus = true;
                        mdb.EmpresaId = e.EmpresaId;
                        mdb.UsuarioId = usuarioId;
                        mdb.Empresas = empresa ?? new Empresas();
                        mdb.Usuarios = usu;
                        mdb.Impuesto = e.Impuesto;
                        if (archivo != null)
                        {
                            mdb.ArchivoId = archivo.Id;
                            mdb.Archivos = archivo;
                        }
                        dbContext.Update(mdb);
                    }
                    else
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No se encuentra el producto que quieres modificar."
                        };
                    }
                }
                else
                {
                    if (!AutenticacionHelper.tienePermiso("Productos", "Agregar", GetPermisos()))
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "No tienes permisos para agregar registros."
                        };

                    e.Estatus = true;
                    e.UsuarioId = usuarioId;
                    e.Empresas = empresa ?? new Empresas();
                    e.Usuarios = usu;
                    e.ArchivoId = archivo != null ? archivo.Id : null;
                    e.Archivos = archivo;
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
        [Authorize("Productos", "Ver")]
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
                var producto = await dbContext.Productos.Where(c => c.Id == Id).Include(c => c.Empresas).Include(c => c.Archivos).FirstOrDefaultAsync();
                if (producto == null)
                {
                    respuesta.IsSuccess = false;
                    respuesta.Message = "No encontre el producto que quieres modificar";
                }
                else
                {
                    respuesta.IsSuccess = true;
                    respuesta.Message = "Información correcta";
                    respuesta.Result = producto;
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
        [Authorize("Productos", "Eliminar")]
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
                var permisos = await dbContext.Productos.Where(c => ids.Contains(c.Id)).ToListAsync();
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
