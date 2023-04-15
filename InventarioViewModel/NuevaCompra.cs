using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioViewModel
{
    public class NuevaCompra
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Total { get; set; }
        public int CompraId { get; set; }
        public int EmpresaId { get; set; }
        public string FacturaCodigo { get; set; }
        public int ProveedorId { get; set; }
        public DateTime FechaCompra { get; set; }
        public int Impuesto { get; set; }
    }
}
