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
    public class DetalleCompraController : BaseController
    {
        public DetalleCompraController(IMailService _mailSender, InventarioDbContext _dbContext) : base(_mailSender, _dbContext)
        {
        }

        [Authorize("Compras", "AgregarModificar")]
        public async Task<IActionResult> Index(int? Id)
        {
            tienePermiso("Compras", new List<RolPermisos>());

            var proveedores = dbContext.Contactos.Where(c => c.Tipo == "Proveedor" && c.Activo == true).ToList();
            ViewBag.Productos = dbContext.Productos.Where(c => c.Estatus == true).OrderBy(c => c.Nombre).ToList();
            ViewBag.Ipuestos = await dbContext.Impuestos.Where(c => c.Activo == true).OrderBy(c => c.AplicarDefault).ToListAsync();
            ViewBag.Empresas = await dbContext.Empresas.Where(c => c.Activo == true).ToListAsync();
            ViewBag.Proveedores = proveedores;

            if (Id == null || Id == 0)
            {
                ViewBag.FacturaId = 0;
                Factura fac = new Factura
                {
                    Codigo = Generic.RandomString(10),
                    CorreoElectronico = ""
                };
                return View(fac);
            }
            else
            {
                ViewBag.FacturaId = Id;
                var comp = dbContext.Compras.Where(c => c.Id == Id).Include(c => c.Contactos).FirstOrDefault();
                Factura fac = new Factura
                {
                    Codigo = comp.CodigoFactura,
                    CorreoElectronico = "",
                    Direccion = comp.Contactos.Direccion,
                    FechaCreacion = comp.Fecha.ToString("dd") + " de " + char.ToUpper(comp.Fecha.ToString("MMM")[0]) + comp.Fecha.ToString("MMM").Substring(1) + " de " + comp.Fecha.Year,
                    ProveedorId = comp.ProveedorId,
                    EmpresaId = comp.EmpresaId,
                    Impuesto = comp.Impuesto,
                    FechaCreacionDate = comp.FechaCompra,
                    Proveedor = comp.Contactos.Nombre,
                    Telefono = comp.Contactos.Telefono,
                    Fecha = comp.FechaCompra.ToString("dd") + " de " + char.ToUpper(comp.FechaCompra.ToString("MMM")[0]) + comp.FechaCompra.ToString("MMM").Substring(1) + " de " + comp.FechaCompra.Year,
                };
                return View(fac);
            }


        }


        [Authorize("Compras", "Ver")]
        public async Task<ActionResult> Lista(int Id)
        {
            tienePermiso("Compras", new List<RolPermisos>());

            int totalItem = 0;
            List<Compra_Productos> productos = new List<Compra_Productos>();
            totalItem = await dbContext.Compra_Productos.Where(c => c.CompraId == Id).CountAsync();
            if (totalItem > 0)
                productos = await dbContext.Compra_Productos.Where(c => c.CompraId == Id).Include(c => c.Productos).Include(c => c.Compras).ToListAsync();
            return PartialView(productos);
        }


        [HttpPost]
        [Authorize("Compras", "AgregarModificar")]
        public async Task<ActionResult<Response>> Save(NuevaCompra cp2)
        {
            try
            {
                if (HttpContext.Response.StatusCode == 401 || HttpContext.Response.StatusCode == 403)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No tienes permisos para realizar esta acción.",
                        Id = cp2.CompraId
                    };
                int usuarioId = AutenticacionHelper.GetUsuario(HttpContext);

                if (!AutenticacionHelper.tienePermiso("Compras", "Agregar", GetPermisos()))
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No tienes permisos para agregar registros.",
                        Id = cp2.CompraId
                    };

                var usuario = dbContext.Usuarios.Where(c => c.Id == usuarioId).FirstOrDefault();
                Compras p = null;
                if (cp2.CompraId == 0)
                {
                    p = new Compras
                    {
                        CodigoFactura = cp2.FacturaCodigo,
                        EmpresaId = cp2.EmpresaId,
                        Estatus = true,
                        Fecha = DateTime.Now,
                        FechaCompra = cp2.FechaCompra,
                        Impuesto = cp2.Impuesto,
                        PorcentajeImpuesto = 0,
                        Neto = 0,
                        ProveedorId = cp2.ProveedorId,
                        Total = 0,
                        UsuarioId = usuarioId
                    };
                    dbContext.Add(p);
                    dbContext.SaveChanges();
                    cp2.CompraId = p.Id;
                }
                else
                {
                    p = dbContext.Compras.Where(c => c.Id == cp2.CompraId).FirstOrDefault();
                }
                var prod = dbContext.Productos.Where(c => c.Id == cp2.ProductoId).FirstOrDefault();
                Compra_Productos cp = new Compra_Productos
                {
                    UsuarioId = usuarioId,
                    Usuarios = usuario,
                    Compras = dbContext.Compras.Where(c => c.Id == cp2.CompraId).FirstOrDefault(),
                    Fecha = DateTime.Now,
                    Cantidad = cp2.Cantidad,
                    CompraId = cp2.CompraId,
                    ProductoId = cp2.ProductoId,
                    Total = cp2.Total,
                    Productos = prod
                };
                prod.Stock += cp2.Cantidad;
                dbContext.Add(cp);
                dbContext.Update(prod);
                await dbContext.SaveChangesAsync();

                var items = dbContext.Compra_Productos.Where(c => c.CompraId == cp2.CompraId).ToList();
                decimal subtotal = 0;
                decimal impuesto = 0;
                decimal total = 0;
                foreach (var item in items)
                {
                    subtotal += item.Total * item.Cantidad;
                }
                impuesto = (subtotal * p.Impuesto) / 100;
                total = subtotal + impuesto;

                p.CodigoFactura = cp2.FacturaCodigo;
                p.EmpresaId = cp2.EmpresaId;
                p.FechaCompra = cp2.FechaCompra;
                p.Impuesto = cp2.Impuesto;
                p.Neto = subtotal;
                p.ProveedorId = cp2.ProveedorId;
                p.Total = total;
                p.UsuarioId = usuarioId;
                p.PorcentajeImpuesto = impuesto;
                dbContext.Update(p);
                await dbContext.SaveChangesAsync();
                return new Response
                {
                    IsSuccess = true,
                    Message = "Se guardo la información correctamente.",
                    Id = cp2.CompraId
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.ToString(),
                    Id = cp2.CompraId
                };
            }
        }


        [HttpGet]
        [Authorize("Compras", "Eliminar")]
        public async Task<ActionResult<Response>> Delete(int Id)
        {
            if (HttpContext.Response.StatusCode == 401 || HttpContext.Response.StatusCode == 403)
                return new Response
                {
                    IsSuccess = false,
                    Message = "No tienes permisos para realizar esta acción."
                };
            try
            {
                var respuesta = new Response();
                var compra = await dbContext.Compra_Productos.Where(c => c.Id == Id).FirstOrDefaultAsync();
                dbContext.Remove(compra);
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
