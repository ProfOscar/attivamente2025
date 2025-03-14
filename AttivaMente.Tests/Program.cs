using AttivaMente.Core.Models;
using AttivaMente.Core.Security;

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

Console.WriteLine($"Hello, {utente}");
