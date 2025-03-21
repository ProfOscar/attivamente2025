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
string dbFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}AttivaMenteDB.mdf";
string connStr = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbFilePath};Integrated Security=True;Connect Timeout=30";
using (SqlConnection connection = new SqlConnection(connStr))
{
    connection.Open();
    // string sqlQuery = "SELECT * FROM Ruoli";
    string sqlQuery = "SELECT Id, Nome FROM Ruoli";
    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
    {
        using (SqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string nome = reader.GetString(1);
                Console.WriteLine($"ID: {id}, Nome: {nome}");
            }
        }
    }
    connection.Close();
}
#endregion

Console.ReadKey();