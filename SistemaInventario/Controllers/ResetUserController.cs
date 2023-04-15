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
    public class ResetUserController : BaseController
    {
        protected IOptions<HashingOptions> options;

        public ResetUserController(IMailService _mailSender, InventarioDbContext _dbContext,
            IOptions<HashingOptions> _options) : base(_mailSender, _dbContext)
        {
            this.options = _options;
        }

        public IActionResult Index(string token)
        {
            ViewBag.isToken = false;
            if (token != null && token.Length > 0)
            {
                DateTime fecha = DateTime.Now.AddHours(-1);
                var usu = dbContext.Tokens.Where(x => x.Token == token && x.Fecha >= fecha).FirstOrDefault();
                if (usu != null)
                {
                    ViewBag.isToken = true;
                    ViewBag.Token = token;
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<Response>> SendInstruction(string user)
        {
            try
            {
                var usu = dbContext.Usuarios.Where(x => x.Email == user).FirstOrDefault();
                if (usu != null)
                {
                    string giud = Guid.NewGuid().ToString();
                    dbContext.Add(new Tokens
                    {
                        Fecha = DateTime.Now,
                        Token = giud,
                        UsuarioId = usu.Id
                    });

                    await dbContext.SaveChangesAsync();

                    string strMensaje = "Gracias por contactar el servicio de cambio de contraseña <br/><br/>" +
                        "Para restablecer tu contraseña, da clic al siguiente link o " +
                        "copialo y pegalo en tu navegador. <br/><br/>" +
                        Generic.GetPath(Request) + "ResetUser?token=" + giud + "<br/><br/>" +
                        "Saludos y gracias.";

                    await mailSender.SendEmailAsync(new MailRequest
                    {
                        Body = strMensaje,
                        Subject = "Restablecer contraseña",
                        ToEmail = user
                    });

                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Se enviaron las instrucciones a tu correo para restablecer tu contraseña."
                    };

                }
                return new Response
                {
                    IsSuccess = false,
                    Message = "No encontre el usuario."
                };
            }
            catch
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "En este momento no puedo enviar las instrucciones, intentalo más tarde por favor."
                };
            }
        }

        [HttpPost]
        public async Task<ActionResult<Response>> Reset(string password, string token)
        {
            try
            {
                DateTime fecha = DateTime.Now.AddHours(-1);
                var usr = dbContext.Tokens.Where(x => x.Token == token && x.Fecha >= fecha).Include(c => c.Usuarios).FirstOrDefault();
                if (usr != null && usr.Usuarios != null)
                {
                    PasswordHasher passwordHasher = new PasswordHasher(options);
                    string pass = passwordHasher.Hash(password);
                    usr.Usuarios.Password = pass;
                    await dbContext.SaveChangesAsync();
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Se actualizó la contraseña correctamente."
                    };
                }
                return new Response
                {
                    IsSuccess = false,
                    Message = "El token que estas usando no existe o esta vencido."
                };

            }
            catch
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "En este momento no puedo actualizar tu contraseña, intentalo más tarde por favor."
                };
            }
        }

    }
}
