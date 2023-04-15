using InventarioHerramienta.Interfaces;
using InventarioModelo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataContext;
using SistemaInventario.Models;
using System.Diagnostics;

namespace SistemaInventario.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(IMailService _mailSender, InventarioDbContext _dbContext) : base(_mailSender, _dbContext)
        {
        }

        public IActionResult Index()
        {
            tienePermiso("Accesos", new List<RolPermisos>());
            int totReg = dbContext.Usuarios.Count();
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