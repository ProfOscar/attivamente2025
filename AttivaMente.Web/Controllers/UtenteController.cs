using AttivaMente.Core.Models;
using AttivaMente.Core.Security;
using AttivaMente.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AttivaMente.Web.Controllers
{
    public class UtenteController : Controller
    {
        private readonly UtenteRepository _repoUtenti;
        private readonly RuoloRepository _repoRuoli;

        public UtenteController(IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("DefaultConnection");
            _repoUtenti = new UtenteRepository(connStr);
            _repoRuoli = new RuoloRepository(connStr);
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

        public IActionResult Create()
        {
            ViewBag.Title = "Aggiungi Utente";
            ViewBag.Ruoli = new SelectList(_repoRuoli.GetAll(), "Id", "Nome");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Utente utente, string Password, string ConfermaPassword)
        {
            if (Password != ConfermaPassword)
            {
                ModelState.AddModelError("", "Le password non coincidono.");
                ViewBag.Ruoli = new SelectList(_repoRuoli.GetAll(), "Id", "Nome");
                return View(utente);
            }
            if (ModelState.IsValid)
            {
                utente.PasswordHash = PasswordHelper.HashPassword(Password);
                _repoUtenti.Add(utente);
                return RedirectToAction("Index");
            }
            ViewBag.Ruoli = new SelectList(_repoRuoli.GetAll(), "Id", "Nome");
            return View(utente);
        }

    }
}
