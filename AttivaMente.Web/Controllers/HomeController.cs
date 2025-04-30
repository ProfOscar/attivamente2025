using AttivaMente.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AttivaMente.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var ruoloRepository = new RuoloRepository(connectionString);

            ViewBag.Title = "Homepage";
            return View(ruoloRepository.GetAll()); // Passa i dati alla View
        }
    }
}
