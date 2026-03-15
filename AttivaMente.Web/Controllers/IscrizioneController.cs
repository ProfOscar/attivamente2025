using AttivaMente.Data;
using AttivaMente.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AttivaMente.Web.Controllers
{
    public class IscrizioneController : Controller
    {
        private readonly IscrizioneRepository _iscrizioneRepository;
        private readonly UtenteRepository _utenteRepository;

        public IscrizioneController(IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("DefaultConnection")!;
            _iscrizioneRepository = new IscrizioneRepository(connStr);
            _utenteRepository = new UtenteRepository(connStr);
        }

        public IActionResult Index(int? anno, bool soloIscritti = true)
        {
            int currentYear = DateTime.Now.Year;
            var years = _iscrizioneRepository.GetYears();

            if (!years.Contains(currentYear))
            {
                years.Insert(0, currentYear);
            }

            years = years
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            int selectedYear = anno ?? years.FirstOrDefault(currentYear);

            var model = new IscrizioniIndexViewModel
            {
                SelectedYear = selectedYear,
                SoloIscritti = soloIscritti,
                Years = years
            };

            if (soloIscritti)
            {
                var iscritti = _iscrizioneRepository.GetByYear(selectedYear);

                foreach (var iscrizione in iscritti)
                {
                    model.Rows.Add(new IscrizioneRowViewModel
                    {
                        UtenteId = iscrizione.UtenteId,
                        Cognome = iscrizione.Utente!.Cognome,
                        Nome = iscrizione.Utente!.Nome,
                        Email = iscrizione.Utente!.Email,
                        Tipo = iscrizione.Tipo,
                        DataIscrizione = iscrizione.DataIscrizione,
                        Azione = "Cancella"
                    });
                }
            }
            else
            {
                var utenti = _utenteRepository.GetAll();
                var iscrizioniAnno = _iscrizioneRepository.GetByYear(selectedYear);

                foreach (var utente in utenti)
                {
                    var iscrizioneAnnoCorrente = iscrizioniAnno.FirstOrDefault(i => i.UtenteId == utente.Id);

                    bool iscrittoAnnoSelezionato = iscrizioneAnnoCorrente != null;
                    bool iscrittoAnnoPrecedente = _iscrizioneRepository.Exists(utente.Id, selectedYear - 1);

                    string azione;
                    string? tipo = null;
                    DateTime? dataIscrizione = null;

                    if (iscrittoAnnoSelezionato)
                    {
                        azione = "Cancella";
                        tipo = iscrizioneAnnoCorrente!.Tipo;
                        dataIscrizione = iscrizioneAnnoCorrente.DataIscrizione;
                    }
                    else if (iscrittoAnnoPrecedente)
                    {
                        azione = "Rinnova";
                    }
                    else
                    {
                        azione = "Iscrivi";
                    }

                    model.Rows.Add(new IscrizioneRowViewModel
                    {
                        UtenteId = utente.Id,
                        Cognome = utente.Cognome,
                        Nome = utente.Nome,
                        Email = utente.Email,
                        Tipo = tipo,
                        DataIscrizione = dataIscrizione,
                        Azione = azione
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int utenteId, int anno, bool soloIscritti)
        {
            _iscrizioneRepository.Delete(utenteId, anno);
            return RedirectToAction("Index", new { anno, soloIscritti });
        }

        [HttpPost]
        public IActionResult Renew(int utenteId, int anno, bool soloIscritti)
        {
            if (!_iscrizioneRepository.Exists(utenteId, anno))
            {
                _iscrizioneRepository.Insert(utenteId, anno, "Rinnovo");
            }

            return RedirectToAction("Index", new { anno, soloIscritti });
        }

        [HttpPost]
        public IActionResult Subscribe(int utenteId, int anno, bool soloIscritti)
        {
            if (!_iscrizioneRepository.Exists(utenteId, anno))
            {
                _iscrizioneRepository.Insert(utenteId, anno, "Nuova");
            }

            return RedirectToAction("Index", new { anno, soloIscritti });
        }
    }
}