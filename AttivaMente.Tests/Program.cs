using AttivaMente.Core.Models;
using AttivaMente.Core.Security;
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
string connStr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\oscar.cambieri\\Desktop\\attivamente2025\\AttivaMente.Data\\AttivaMenteDB.mdf;Integrated Security=True;Connect Timeout=30";
using (SqlConnection connection = new SqlConnection(connStr))
{
    string sqlQuery = "SELECT * FROM Utenti";
    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
    {

    }
}
#endregion