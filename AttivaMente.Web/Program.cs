using AttivaMente.Core.Models;
using AttivaMente.Core.Security;

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
