using AttivaMente.Core.Models;
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
            ViewBag.Title = "Ruoli";
            var ruoli = _repo.GetAll();
            return View(ruoli);
        }


        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Ruolo ruolo)
        {
            if (ModelState.IsValid)
            {
                _repo.Add(ruolo);
                return RedirectToAction("Index");
            }
            return View(ruolo);
        }


        public IActionResult Edit(int id)
        {
            var ruolo = _repo.GetById(id);
            if (ruolo == null) return NotFound();
            return View(ruolo);
        }

        [HttpPost]
        public IActionResult Edit(Ruolo ruolo)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(ruolo);
                return RedirectToAction("Index");
            }
            return View(ruolo);
        }
    }
}
