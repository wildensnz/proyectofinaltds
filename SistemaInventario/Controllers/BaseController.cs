using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DataContext;

namespace SistemaInventario.Controllers
{
    public class BaseController : Controller
    {

        protected readonly InventarioDbContext dbContext;

        public BaseController(InventarioDbContext _dbContext)
        {
            dbContext = _dbContext;  
        }
    }
}
