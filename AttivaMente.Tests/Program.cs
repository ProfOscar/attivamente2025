using AttivaMente.Core.Models;
using AttivaMente.Core.Security;

Utente utente = new Utente()
{
    Nome = "Mario",
    Cognome = "Rossi",
    Email = "mario.rossi@example.com",
    PasswordHash = PasswordHelper.HashPassword("miapassword123"),
    RuoloId = 1
};

Console.WriteLine("Hello, World!");
