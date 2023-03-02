using InventarioViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataContext;
using System.Security.Claims;

namespace SistemaInventario.Controllers
{
    public class LoginController : BaseController
    {
        public LoginController(InventarioDbContext _dbContext) : base(_dbContext)
        {
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult<Response>> Validate(string user, string password, string ReturnUrl, bool RememberLogin = false)
        {
            var valiUser = await dbContext.Usuarios.Where(c => c.Email == user && c.Clave == password).FirstOrDefaultAsync();
            if (valiUser == null)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "El usuario y/o contraseña son incorrectos"
                };
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, Convert.ToString(valiUser.ID)),
                new Claim(ClaimTypes.Name, valiUser.Nombre),
                new Claim(ClaimTypes.Role, valiUser.Roles.Rol),
                new Claim("Estatus", valiUser.Estatus.ToString()),  
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
            {
                IsPersistent = RememberLogin
            });

            return new Response
            {
                IsSuccess = true,
                Url = ReturnUrl
            };

        }
    }
}
