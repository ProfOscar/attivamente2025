using Microsoft.AspNetCore.Mvc;

namespace AttivaMente.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
