using AttivaMente.Core.Models;
using AttivaMente.Core.Security;
using AttivaMente.Data;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Aggiunge i servizi MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Abilita i file statici (CSS, JS, immagini)
app.UseStaticFiles();


#region TestCreazioneUtente
//Ruolo rAdmin = new Ruolo()
//{
//    Nome = "Admin"
//};

//Utente utente = new Utente()
//{
//    Nome = "Mario",
//    Cognome = "Rossi",
//    Email = "mario.rossi@example.com",
//    PasswordHash = PasswordHelper.HashPassword("miapassword123"),
//    RuoloId = 0,
//    Ruolo = rAdmin
//};

//app.MapGet("/", () => $"Hello {utente}");
#endregion

#region TestConnessioneDB
//string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//Database db = new Database(connectionString);

//try
//{
//    using var conn = db.GetConnection();
//    conn.Open();
//    Console.WriteLine("Connessione riuscita.");
//}
//catch (Exception ex)
//{
//    Console.WriteLine("Errore di connessione: " + ex.Message);
//}
#endregion

// Configura il routing: usa i Controller e le Views
app.UseRouting();

// Inizialmente non lo usiamo
//app.UseAuthorization();

// Imposta la route predefinita per i Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
