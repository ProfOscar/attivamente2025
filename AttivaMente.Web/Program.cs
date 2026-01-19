using AttivaMente.Core.Models;
using AttivaMente.Core.Security;
using AttivaMente.Data;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Aggiunge i servizi MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

var app = builder.Build();


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

// Variabili controllo DB
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
string dbPath = connectionString.Split(';')[1].Replace("AttachDbFilename=", "");
string schemaPath = Path.GetFullPath(builder.Configuration["Database:SchemaPath"]);
string seedDataPath = Path.GetFullPath(builder.Configuration["Database:SeedDataPath"]);
string migrationsPath = Path.GetFullPath(builder.Configuration["Database:MigrationsPath"]);

// Log posizione DB e scripts
Console.WriteLine($"[AttivaMente] DB path: {dbPath}");
Console.WriteLine($"[AttivaMente] Schema SQL path: {schemaPath}");
Console.WriteLine($"[AttivaMente] Seed Data  path: {seedDataPath}");
Console.WriteLine($"[AttivaMente] Migrations path: {migrationsPath}");

var db = new Database(connectionString);

// Inizializza DB se assente
db.EnsureDatabaseCreated(dbPath, schemaPath);

// Seed dati iniziali
db.InitialDataSeed(seedDataPath);

// TODO: Gestione migrazioni DB
db.ApplyMigrations(migrationsPath);

// Abilita i file statici (CSS, JS, immagini)
app.UseStaticFiles();

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
