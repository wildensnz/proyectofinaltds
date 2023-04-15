using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SistemaInventario.DataContext;
using SistemaInventario.Helpers;
using System.Net;

namespace SistemaInventario.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(params string[] moduloAccion) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { moduloAccion };
        }
    }

    public class AuthorizeFilter : IAuthorizationFilter
    {
        readonly string[] moduloAccion;
        protected readonly InventarioDbContext dbContext;

        public AuthorizeFilter(InventarioDbContext _dbContext, params string[] moduloAccion)
        {
            this.moduloAccion = moduloAccion;
            dbContext = _dbContext;
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var IsAuthenticated = context.HttpContext.User.Identity == null ? false : context.HttpContext.User.Identity.IsAuthenticated;
            //var claimsIndentity = context.HttpContext.User.Identity as ClaimsIdentity;
            //context.Result = new RedirectResult("~/Login");
            if (IsAuthenticated)
            {
                int rolId = AutenticacionHelper.GetValueClaim(context.HttpContext, "RolId");
                var permisos = dbContext.RolPermisos.Where(c => c.Modulo == moduloAccion[0] &&
                c.RolId == rolId && c.Activo == true).ToList();

                if (!AutenticacionHelper.tienePermiso(moduloAccion[0], moduloAccion[1], permisos))
                {
                    if (context.HttpContext.Request.IsAjaxRequest())
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    else
                        context.Result = new RedirectResult("~/Error/AccesoDenegado");
                }
            }
            else
            {
                if (context.HttpContext.Request.IsAjaxRequest())
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }
                else
                {
                    context.Result = new RedirectResult("~/Login");
                }
            }
            return;
        }
    }
}
