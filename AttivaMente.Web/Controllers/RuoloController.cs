using AttivaMente.Data;
using Microsoft.AspNetCore.Mvc;

namespace AttivaMente.Web.Controllers
{
    public class RuoloController : Controller
    {
        private readonly RuoloRepository _repo;
        
        public RuoloController(IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("DefaultConnection");
            _repo = new RuoloRepository(connStr);
        }

        public IActionResult Index()
        {
            var ruoli = _repo.GetAll();
            return View(ruoli);
        }
    }
}
