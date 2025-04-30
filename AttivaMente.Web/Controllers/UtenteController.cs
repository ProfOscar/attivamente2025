using Microsoft.AspNetCore.Mvc;

namespace AttivaMente.Web.Controllers
{
    public class UtenteController : Controller
    {
        public UtenteController(IConfiguration configuration)
        {

        }

        public IActionResult Index()
        {
            ViewBag.Title = "Utenti";
            return Content("TODO::Gestione Utenti");
        }
    }
}
