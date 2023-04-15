using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioModelo
{
    public class Empresas { 

        [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public DateTime Fecha { get; set; }
    public int UsuarioId { get; set; }
    public string CorreoElectronico { get; set; } = "";
    public string Telefono { get; set; } = "";
    public int? MonedaId { get; set; }
    public int? LogoId { get; set; }
    public bool Activo { get; set; }
    public string? Calle { get; set; } = "";
    public string? Ciudad { get; set; } = "";
    public string? Estado { get; set; } = "";
    public string? CodigoPostal { get; set; } = "";
    public int PaisId { get; set; }
    public string? Descripcion { get; set; } = "";
    public int TipoEmpresaId { get; set; }

    [ForeignKey("TipoEmpresaId")]
    public virtual Catalogos TipoEmpresa { get; set; } = new Catalogos();

    [ForeignKey("PaisId")]
    public virtual Catalogos Pais { get; set; } = new Catalogos();

    public virtual IEnumerable<Compras> Compras { get; set; } = new List<Compras>();
    public virtual IEnumerable<Contactos> Contactos { get; set; } = new List<Contactos>();

    [ForeignKey("UsuarioId")]
    public virtual Usuarios Usuarios { get; set; } = new Usuarios();

    [ForeignKey("LogoId")]
    public virtual Archivos Logo { get; set; } = new Archivos();
    public virtual IEnumerable<Productos> Productos { get; set; } = new List<Productos>();
    public virtual IEnumerable<Ventas> Ventas { get; set; } = new List<Ventas>();
}
}
