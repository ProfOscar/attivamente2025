using AttivaMente.Data;
using AttivaMente.Web.ViewModels;
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

        public IActionResult Index(int? anno, bool isSoloIscritti = true, string? ricerca = null)
        {
            int currentYear = DateTime.Now.Year;
            var years = _repoIscrizioni.GetYears();

            if (!years.Contains(currentYear))
                years.Insert(0, currentYear);

            // Questa riga sistema eventuali incongruenze a db (iscrizioni in anni futuri)
            years = years.Distinct().OrderByDescending(x => x).ToList();

            int selectedYear = anno ?? currentYear;

            var model = new IscrizioniIndexViewModel
            {
                SelectedYear = selectedYear,
                IsSoloIscritti = isSoloIscritti,
                Ricerca = ricerca ?? "",
                Years = years
            };

            if (isSoloIscritti)
            {
                var iscritti = _repoIscrizioni.GetByYear(selectedYear);
                foreach (var item in iscritti)
                {
                    model.Rows.Add(new IscrizioneRowViewModel
                    {
                        UtenteId = item.UtenteId,
                        Cognome = item.Utente!.Cognome,
                        Nome = item.Utente!.Nome,
                        Email = item.Utente!.Email,
                        Tipo = item.Tipo,
                        DataIscrizione = item.DataIscrizione,
                        Azione = "Cancella"
                    });
                }
            }
            else
            {
                var utenti = _repoUtenti.GetAll();
                var iscrizioniAnno = _repoIscrizioni.GetByYear(selectedYear);
                foreach (var item in utenti)
                {
                    var iscrizioneAnnoCorrente = iscrizioniAnno.FirstOrDefault(i => i.UtenteId == item.Id);
                    bool isIscrittoAnnoSelezionato = iscrizioneAnnoCorrente != null;
                    bool isIscrittoAnnoPrecedente = _repoIscrizioni.Exists(item.Id, selectedYear - 1);

                    string azione;
                    string? tipo = null;
                    DateTime? dataIscrizione = null;

                    if (isIscrittoAnnoSelezionato)
                    {
                        azione = "Cancella";
                        tipo = iscrizioneAnnoCorrente!.Tipo;
                        dataIscrizione = iscrizioneAnnoCorrente.DataIscrizione;
                    }
                    else if (isIscrittoAnnoPrecedente)
                        azione = "Rinnova";
                    else
                        azione = "Iscrivi";

                    model.Rows.Add(new IscrizioneRowViewModel
                    {
                        UtenteId = item.Id,
                        Cognome = item.Cognome,
                        Nome = item.Nome,
                        Email = item.Email,
                        Tipo = tipo,
                        DataIscrizione = dataIscrizione,
                        Azione = azione
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(ricerca))
            {
                string testo = ricerca.Trim().ToLower();

                model.Rows = model.Rows
                    .Where(r =>
                        r.Nome.ToLower().Contains(testo) ||
                        r.Cognome.ToLower().Contains(testo) ||
                        $"{r.Cognome} {r.Nome}".ToLower().Contains(testo) ||
                        $"{r.Nome} {r.Cognome}".ToLower().Contains(testo))
                    .ToList();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int utenteId, int anno, bool isSoloIscritti, string? ricerca = null)
        {
            _repoIscrizioni.Delete(utenteId, anno);
            return RedirectToAction("Index", new { anno, isSoloIscritti, ricerca });
        }

        [HttpPost]
        public IActionResult Renew(int utenteId, int anno, bool isSoloIscritti, string? ricerca = null)
        {
            if (!_repoIscrizioni.Exists(utenteId, anno))
            {
                _repoIscrizioni.Insert(utenteId, anno, "Rinnovo");
            }
            return RedirectToAction("Index", new { anno, isSoloIscritti, ricerca });
        }

        [HttpPost]
        public IActionResult Subscribe(int utenteId, int anno, bool isSoloIscritti, string? ricerca = null)
        {
            if (!_repoIscrizioni.Exists(utenteId, anno))
            {
                _repoIscrizioni.Insert(utenteId, anno, "Nuova");
            }
            return RedirectToAction("Index", new { anno, isSoloIscritti, ricerca });
        }
    }
}
