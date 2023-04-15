using InventarioHerramienta;
using InventarioHerramienta.Interfaces;
using InventarioViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SistemaInventario.DataContext;
using System.Security.Claims;



namespace SistemaInventario.Controllers
{
    public class LoginController : BaseController
    {
        protected IOptions<HashingOptions> options;

        public LoginController(IMailService _mailSender, InventarioDbContext _dbContext,
            IOptions<HashingOptions> _options) : base(_mailSender, _dbContext)
        {
            this.options = _options;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<Response>> Validate(InventarioViewModel.Login login)
        {

            var valiUser = await dbContext.Usuarios.Where(c => c.Email == login.user).Include(c => c.Roles).FirstOrDefaultAsync();
            if (valiUser == null)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "El usuario no esta registrado en el sistema."
                };
            }


            PasswordHasher passwordHasher = new PasswordHasher(options);
            var checar = passwordHasher.Check(valiUser.Password, login.password);
            if (checar.Verified == false)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "El usuario y/o contraseña son incorrectos"
                };
            }

            var claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, Convert.ToString(valiUser.Id)),
                        new Claim(ClaimTypes.Name, valiUser.Nombre),
                        new Claim(ClaimTypes.Role, valiUser.Roles.Rol),
                        new Claim("Estatus", valiUser.Estatus.ToString()),
                        new Claim("Mail", valiUser.Email.ToString()),
                         new Claim("UsuarioId", valiUser.Id.ToString()),
                         new Claim("RolId", valiUser.Roles.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
            {
                IsPersistent = login.rememberLogin
            });

            return new Response
            {
                IsSuccess = true,
                Url = login.returnUrl
            };

        }

        public async Task<IActionResult> LogOut()
        {
            //SignOutAsync is Extension method for SignOut    
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Redirect to home page    
            return LocalRedirect("/Login");
        }
    }
}
