using InventarioModelo;
using Microsoft.AspNetCore.Mvc;

namespace SistemaInventario.Helpers
{
    public class AutenticacionHelper
    {
        public static int GetUsuario(HttpContext currentUser)
        {
            int UsuarioId = 0;
            try
            {
                if (currentUser.User.HasClaim(c => c.Type == "UsuarioId"))
                {
                    int.TryParse(currentUser.User.Claims.FirstOrDefault(c => c.Type == "UsuarioId")?.Value, out UsuarioId);
                }
            }
            catch
            {

            }
            return UsuarioId;
        }

        public static int GetValueClaim(HttpContext currentUser, string clave)
        {
            int respuesta = 0;
            try
            {
                if (currentUser.User.HasClaim(c => c.Type == clave))
                {
                    int.TryParse(currentUser.User.Claims.FirstOrDefault(c => c.Type == clave)?.Value, out respuesta);
                }
            }
            catch
            {

            }
            return respuesta;
        }

        public static bool tienePermiso(string modulo, string accion, List<RolPermisos> permisos)
        {
            int permiso = 0;
            switch (accion)
            {
                case "Ver":
                    permiso = permisos.Where(c => c.Modulo == modulo && c.Ver == true).Count();
                    break;
                case "Agregar":
                    permiso = permisos.Where(c => c.Modulo == modulo && c.Agregar == true).Count();
                    break;
                case "Modificar":
                    permiso = permisos.Where(c => c.Modulo == modulo && c.Modificar == true).Count();
                    break;
                case "Eliminar":
                    permiso = permisos.Where(c => c.Modulo == modulo && c.Eliminar == true).Count();
                    break;
                case "AgregarModificar":
                    permiso = permisos.Where(c => c.Modulo == modulo && (c.Agregar == true || c.Modificar == true)).Count();
                    break;
                default:
                    permiso = 0;
                    break;
            }
            return permiso == 0 ? false : true;

        }
    }
}
