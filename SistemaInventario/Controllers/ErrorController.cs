using Microsoft.AspNetCore.Mvc;

namespace SistemaInventario.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
