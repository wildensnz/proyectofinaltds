using InventarioHerramienta;
using InventarioHerramienta.Interfaces;
using InventarioModelo;
using InventarioViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SistemaInventario.DataContext;

namespace SistemaInventario.Controllers
{
    public class RegisterController : BaseController
    {
        protected IOptions<HashingOptions> options;
        public RegisterController(IMailService _mailSender, InventarioDbContext _dbContext, IOptions<HashingOptions> _options) : base(_mailSender, _dbContext)
        {
            this.options = _options;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task <ActionResult<Response>> Now(InventarioViewModel.Login login)
        {
            try
            {
                var valiUser = await dbContext.Usuarios.Where(c => c.Email == login.user).FirstOrDefaultAsync();
                if (valiUser == null)
                {
                    PasswordHasher passwordHasher = new PasswordHasher(options);
                    string pass = passwordHasher.Hash(login.password);
                    Usuarios us = new Usuarios
                    {
                        Email = login.user,
                        Estatus = true,
                        Nombre = login.name,
                        Password = pass,
                        RolId= 1
                    };

                    dbContext.Add(us);
                    await dbContext.SaveChangesAsync();
                    if(us.Id > 0)
                    {
                        return new Response
                        {
                            IsSuccess = true,
                            Message = "El usuario se creo correctamente."
                        };
                    }
                }
            

                return new Response
                {
                    IsSuccess = false,
                    Message = "El usuario ya se encuentra registrado."
                };

            }
            catch
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "No se ha podido registrar en el sistema, debe estar ocurriendo un error"
                };
            }
        }

    }
}
