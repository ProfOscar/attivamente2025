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
            ViewBag.Title = $"Dettaglio Utente {id}";
            var utente = _repoUtenti.GetById(id);
            return utente == null ? NotFound() : View(utente);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Aggiungi Utente";
            ViewBag.SelectRuoli = new SelectList(_repoRuoli.GetAll(), "Id", "Nome");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Utente utente, string password, string confermaPassword)
        {
            if (password != confermaPassword)
            {
                ModelState.AddModelError("password", "Le password non coincidono.");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    utente.PasswordHash = PasswordHelper.HashPassword(password);
                    _repoUtenti.Add(utente);
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Title = "Aggiungi Utente";
            ViewBag.SelectRuoli = new SelectList(_repoRuoli.GetAll(), "Id", "Nome");
            return View(utente);
        }
    }
}
