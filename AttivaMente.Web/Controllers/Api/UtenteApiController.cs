using AttivaMente.Data;
using Microsoft.AspNetCore.Mvc;

namespace AttivaMente.Web.Controllers.Api
{
    [ApiController]
    [Route("api/utenti")]
    public class UtenteApiController : ControllerBase
    {
        private readonly UtenteRepository _repoUtenti;
        public UtenteApiController(IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("DefaultConnection");
            _repoUtenti = new UtenteRepository(connStr);
        }

        [Route("all")]
        public IActionResult Index()
        {
            var utenti = _repoUtenti.GetAll();
            return Ok(utenti);
        }
    }
}
