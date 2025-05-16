using AttivaMente.Data;
using Microsoft.AspNetCore.Mvc;

namespace AttivaMente.Web.Controllers
{
    public class UtenteController : Controller
    {
        private readonly UtenteRepository _repoUtenti;

        public UtenteController(IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("DefaultConnection");
            _repoUtenti = new UtenteRepository(connStr);
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Utenti";
            var utenti = _repoUtenti.GetAll();
            return View(utenti);
        }

        public IActionResult Details(int id)
        {
            var utente = _repoUtenti.GetById(id);
            return utente == null ? NotFound() : View(utente);
        }
    }
}
