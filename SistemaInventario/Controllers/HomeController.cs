using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DataContext;
using SistemaInventario.Models;
using System.Diagnostics;

namespace SistemaInventario.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InventarioDbContext dbContext;

        public HomeController(ILogger<HomeController> logger, InventarioDbContext _dbContext)
        {
            _logger = logger;
            dbContext= _dbContext;
        }

        public IActionResult Index()
        {
            //int countUser = dbContext.Usuarios.Count();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}