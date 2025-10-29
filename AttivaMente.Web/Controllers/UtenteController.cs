using AttivaMente.Core.Models;
using AttivaMente.Core.OfficeAutomation;
using AttivaMente.Core.Security;
using AttivaMente.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

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

        public IActionResult Index(
            string? searchTerm, 
            int? ruoloFilter, 
            string? orderBy, 
            string? direction,
	        int page = 1,
	        int pageSize = 10)
		{
            ViewBag.Title = "Utenti";
            ViewBag.SearchTerm = searchTerm;
            ViewBag.RuoloFilter = ruoloFilter;
            ViewBag.OrderBy = orderBy;
            ViewBag.Direction = direction;
			ViewBag.Page = page;
			ViewBag.PageSize = pageSize;

			var ruoli = _repoRuoli.GetAll();
            ViewBag.RuoliSelectList = new SelectList(ruoli, "Id", "Nome");

            List<Utente> utenti = string.IsNullOrWhiteSpace(searchTerm) && (ruoloFilter == 0) 
                ? _repoUtenti.GetAll() 
                : _repoUtenti.Search(searchTerm!, ruoloFilter, orderBy, direction, page, pageSize);
			
            int total = _repoUtenti.Count(searchTerm, ruoloFilter);
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

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

        public IActionResult Edit(int id)
        {
            var utente = _repoUtenti.GetById(id);
            if (utente == null) return NotFound();
            ViewBag.Title = "Modifica Utente";
            ViewBag.SelectRuoli = new SelectList(_repoRuoli.GetAll(), "Id", "Nome", utente.RuoloId);
            return View(utente);
        }

        [HttpPost]
        public IActionResult Edit(Utente utente)
        {
            if (ModelState.IsValid)
            {
                _repoUtenti.Update(utente);
                return RedirectToAction("Index");
            }
            ViewBag.Title = "Modifica Utente";
            ViewBag.SelectRuoli = new SelectList(_repoRuoli.GetAll(), "Id", "Nome", utente.RuoloId);
            return View(utente);
        }

        public IActionResult Delete(int id)
        {
            var utente = _repoUtenti.GetById(id);
            if (utente == null) return NotFound();
            return View(utente);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repoUtenti.Delete(id);
            return RedirectToAction("Index");
        }

        public IActionResult CreateDocx(int id)
        {
            var utente = _repoUtenti.GetById(id);
            if (utente == null) return NotFound();
            ViewBag.Title = "Scarica DOCX Utente";
			byte[] fileBytes = WordAutomation.GetUserDocxBytes(utente, "C:\\Dati\\contact.docx"); ;
            return File(
                fileBytes, 
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"{utente.Cognome}_{utente.Nome}_{utente.Id}.docx");
        }

        public IActionResult CreateXlsx(string? searchTerm, int? ruoloFilter, string? orderBy, string? direction)
        {
			var utenti = string.IsNullOrWhiteSpace(searchTerm) && (ruoloFilter == 0)
				? _repoUtenti.GetAll()
				: _repoUtenti.Search(searchTerm!, ruoloFilter, orderBy, direction);
            if (utenti != null && utenti.Count > 0)
            {
                ViewBag.Title = "Scarica XLSX Utente";
                byte[] fileBytes = ExcelAutomation.GetUsersXlsxBytes(utenti);
                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Utenti_{DateTime.Now:yyyyMMdd}.xlsx");
            } else
            {
                return RedirectToAction("Index");
            }
        }
    }
}
