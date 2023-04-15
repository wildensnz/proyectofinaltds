using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioViewModel
{
    public class Factura
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = "";
        public DateTime FechaCreacionDate { get; set; } = DateTime.Now;
        public string Fecha { get; set; } = DateTime.Now.ToString("dd") + " de " + char.ToUpper(DateTime.Now.ToString("MMM")[0]) + DateTime.Now.ToString("MMM").Substring(1) + " de " + DateTime.Now.Year;
        public string FechaCreacion { get; set; } = DateTime.Now.ToString("dd") + " de " + char.ToUpper(DateTime.Now.ToString("MMM")[0]) + DateTime.Now.ToString("MMM").Substring(1) + " de " + DateTime.Now.Year + " " + DateTime.Now.ToString("hh:mm tt");
        public string Proveedor { get; set; } = "";
        public string Direccion { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string CorreoElectronico { get; set; } = "";
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public decimal PorcentajeImpuesto { get; set; }
        public int ProveedorId { get; set; }
        public int EmpresaId { get; set; }
        public List<DetalleFactura> detalleFacturas { get; set; } = new List<DetalleFactura>();

        public class DetalleFactura
        {
            public int Id { get; set; }
            public string Descripcion { get; set; } = "";
            public string Precio { get; set; } = "";
            public decimal Cantidad { get; set; }
            public decimal Costo { get; set; }
        }
    }
}
