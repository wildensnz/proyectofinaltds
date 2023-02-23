using Microsoft.AspNetCore.Mvc;

namespace SistemaInventario.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
