using AttivaMente.Data;
using Microsoft.AspNetCore.Mvc;

namespace AttivaMente.Web.Controllers
{
    public class IscrizioneController : Controller
    {
        private readonly IscrizioneRepository _repoIscrizioni;
        private readonly UtenteRepository _repoUtenti;

        public IscrizioneController(IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("DefaultConnection");
            _repoIscrizioni = new IscrizioneRepository(connStr);
            _repoUtenti = new UtenteRepository(connStr);
        }

        public IActionResult Index(int? anno, string isSoloIscritti = "true")
        {
            var years = _repoIscrizioni.GetYears();
            int selected = anno ?? (years.Count > 0 ? years[0] : DateTime.Now.Year);

            ViewBag.Years = years;
            ViewBag.SelectedYear = selected;
            ViewBag.IsSoloIscritti = isSoloIscritti;

            var iscritti = isSoloIscritti == "true" ? _repoIscrizioni.GetByYear(selected) : _repoIscrizioni.GetAll();
            return View(iscritti);
        }

        [HttpPost]
        public IActionResult Renew(int utenteId, int anno)
        {
            _repoIscrizioni.Renew(utenteId, anno);

            return RedirectToAction("Index", new { anno });
        }
    }
}
