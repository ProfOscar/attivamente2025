using AttivaMente.Core.Models;
using AttivaMente.Core.Security;
using AttivaMente.Data;
using Microsoft.Data.SqlClient;

#region InMemory
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

Console.WriteLine($"Utilizzo utente creato in memoria\n{utente}\n\n");
#endregion

#region SqlServer
string dbFilePath = "C:\\Dati\\AttivaMenteDB.mdf";
string connStr = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbFilePath};Integrated Security=True;Connect Timeout=30";
RuoloRepository repository = new RuoloRepository(connStr);

char scelta = ' ';
do
{
    Console.Clear();
    Console.WriteLine("*** TEST FUNZIONALITA ***");
    Console.WriteLine("\n- RUOLI -");
    Console.WriteLine("a) LISTA ruoli");
    Console.WriteLine("b) ruolo SINGOLO");
    Console.WriteLine("c) AGGIUNGI ruolo");
    Console.WriteLine("d) MODIFICA ruolo");
    Console.WriteLine("e) CANCELLA ruolo");
    Console.WriteLine("\nq) ESCI");
    scelta = Console.ReadKey(true).KeyChar;
    switch (scelta)
    {
        case 'a':
            ListaRuoli();
            break;
        case 'b':
            RuoloSingolo(2);
            break;
        case 'c':
            break;
        case 'd':
            break;
        case 'e':
            break;
        default:
            if (scelta != 'q') 
                Console.WriteLine("\nInserisci una scelta valida");
            break;
    }
    if (scelta != 'q')
    {
        Console.WriteLine("(premi un tasto per continuare)");
        Console.ReadKey(true);
    }
} while (scelta != 'q');

void ListaRuoli()
{
    foreach (var ruolo in repository.GetAll())
    {
        Console.WriteLine($"{ruolo.Id} - {ruolo.Nome}");
    }
}

void RuoloSingolo(int id)
{
    Ruolo ruolo = repository.GetById(id);
    if (ruolo == null)
        Console.WriteLine("NON TROVATO");
    else
        Console.WriteLine(ruolo);
}
#endregion
