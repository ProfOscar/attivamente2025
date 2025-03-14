using AttivaMente.Core.Models;
using AttivaMente.Core.Security;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Ruolo rAdmin = new Ruolo()
{
    Nome = "Admin"
};

Utente utente = new Utente()
{
    Nome = "Mario",
    Cognome = "Rossi",
    Email = "mario.rossi@example.com",
    PasswordHash = PasswordHelper.HashPassword("miapassword123"),
    RuoloId = 0,
    Ruolo = rAdmin
};


app.MapGet("/", () => $"Hello {utente}");
app.MapGet("admin/", () => "Hello World Admin Page!");

app.Run();
