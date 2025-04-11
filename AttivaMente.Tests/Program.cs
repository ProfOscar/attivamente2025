using AttivaMente.Core.Models;
using AttivaMente.Core.Security;
using AttivaMente.Data;

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

RuoloRepository ruoloRepository = new RuoloRepository(connStr);
UtenteRepository utenteRepository = new UtenteRepository(connStr);

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
    Console.WriteLine("\n- UTENTI -");
    Console.WriteLine("g) LISTA utenti");
    Console.WriteLine("h) utente SINGOLO");
    Console.WriteLine("i) AGGIUNGI utente");
    Console.WriteLine("l) MODIFICA utente");
    Console.WriteLine("m) CANCELLA utente");
    Console.WriteLine("-----");
    Console.WriteLine("\nq) ESCI");
    Console.WriteLine();
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
            AggiungiRuolo("Test Aggiunta");
            break;
        case 'd':
            ModificaRuolo("Test Modifica", 5);
            break;
        case 'e':
            CancellaRuolo();
            break;
        case 'g':
            ListaUtenti();
            break;
        case 'h':
            UtenteSingolo(2);
            break;
        case 'i':
            AggiungiUtente();
            break;
        case 'l':
            ModificaUtente();
            break;
        case 'm':
            CancellaUtente();
            break;
        default:
            if (scelta != 'q') 
                Console.WriteLine("\nInserisci una scelta valida");
            break;
    }
    if (scelta != 'q')
    {
        Console.WriteLine("\n(premi un tasto per continuare)");
        Console.ReadKey(true);
    }
} while (scelta != 'q');

void ListaRuoli()
{
    foreach (var ruolo in ruoloRepository.GetAll())
    {
        Console.WriteLine($"{ruolo.Id} - {ruolo.Nome}");
    }
}

void RuoloSingolo(int id)
{
    Ruolo? ruolo = ruoloRepository.GetById(id);
    if (ruolo == null)
        Console.WriteLine("NON TROVATO");
    else
        Console.WriteLine(ruolo);
}

void AggiungiRuolo(string nomeRuolo)
{
    int retVal = ruoloRepository.Add(nomeRuolo);
    if (retVal <= 0)
        Console.WriteLine("NON AGGIUNTO");
    else 
        Console.WriteLine($"Ruolo {nomeRuolo} AGGIUNTO correttamente");
}

void ModificaRuolo(string nomeRuolo, int idRuolo)
{
    int retVal = ruoloRepository.Update(nomeRuolo, idRuolo);
    if (retVal <= 0)
        Console.WriteLine("NON MODIFICATO");
    else
        Console.WriteLine($"Ruolo {nomeRuolo} MODIFICATO correttamente");
}

void CancellaRuolo()
{
    Console.Write("\nInserisci l'Id del Ruolo da CANCELLARE: ");
    int idRuolo = int.Parse(Console.ReadLine() ?? "0");
    int retVal = ruoloRepository.Delete(idRuolo);
    if (retVal <= 0)
        Console.WriteLine("NON CANCELLATO");
    else
        Console.WriteLine($"Ruolo con Id {idRuolo} CANCELLATO correttamente");
}

void ListaUtenti()
{
    foreach (var utente in utenteRepository.GetAll())
    {
        Console.WriteLine(utente);
    }
}

void UtenteSingolo(int id)
{
    Utente? utente = utenteRepository.GetById(id);
    if (utente == null)
        Console.WriteLine("NON TROVATO");
    else
        Console.WriteLine(utente);
}

void AggiungiUtente()
{
    Utente nuovoUtente = new Utente
    {
        Nome = "Guglielmo",
        Cognome = "Marconi",
        Email = "g.marconi@example.com",
        PasswordHash = PasswordHelper.HashPassword("gmarc123"),
        RuoloId = 2
    };
    int retVal = utenteRepository.Add(nuovoUtente);
    if (retVal <= 0)
        Console.WriteLine("NON AGGIUNTO");
    else
        Console.WriteLine($"Utente {nuovoUtente.Cognome} AGGIUNTO correttamente");
}

void ModificaUtente()
{
    Utente? utenteDaModificare = utenteRepository.GetById(2);
    if (utenteDaModificare != null)
    {
        utenteDaModificare.Cognome = "CognModificato";
        utenteDaModificare.Nome = "NomeModificato";
        int retVal = utenteRepository.Update(utenteDaModificare);
        if (retVal <= 0)
            Console.WriteLine("NON MODIFICATO");
        else
            Console.WriteLine($"Utente {utenteDaModificare.Cognome} MODIFICATO correttamente");
    }
    else
        Console.WriteLine("Utente da modificare NON TROVATO");
}

void CancellaUtente()
{
    Console.Write("\nInserisci l'Id dell'Utente da CANCELLARE: ");
    int idUtente = int.Parse(Console.ReadLine() ?? "0");
    int retVal = utenteRepository.Delete(idUtente);
    if (retVal <= 0)
        Console.WriteLine("NON CANCELLATO");
    else
        Console.WriteLine($"Utente con Id {idUtente} CANCELLATO correttamente");
}
#endregion
