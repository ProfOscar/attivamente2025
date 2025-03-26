using AttivaMente.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AttivaMente.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var db = new Database(connectionString);
            var ruoli = new List<string>();

            using var conn = db.GetConnection();
            conn.Open();

            var command = new SqlCommand("SELECT Nome FROM Ruoli", conn);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ruoli.Add(reader.GetString(0));
            }

            return View(ruoli); // Passa i dati alla View
        }
    }
}
