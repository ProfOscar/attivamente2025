using AttivaMente.Core.Models;
using Microsoft.Data.SqlClient;

namespace AttivaMente.Data
{
    public class IscrizioneRepository
    {
        private readonly Database _db;

        public IscrizioneRepository(string connStr) => _db = new Database(connStr);

        public List<int> GetYears() 
        {
            var years = new List<int>();
            string query = "SELECT DISTINCT Anno FROM Iscrizioni ORDER BY Anno DESC";
            using var reader = _db.ExecuteReader(query);
            while (reader.Read())
                years.Add(reader.GetInt32(0));
            return years;
        }

        public List<Iscrizione> GetByYear(int anno)
        {
            var list = new List<Iscrizione>();
            string query = @"
                SELECT  i.Id, i.UtenteId, i.Anno, i.Tipo, i.DataIscrizione,
                        u.Id, u.Nome, u.Cognome, u.Email
                FROM Iscrizioni i
                INNER JOIN Utenti u ON i.UtenteId = u.Id
                WHERE i.Anno = @anno
                ORDER BY u.Cognome, u.Nome";
            var parameters = new[] { new SqlParameter("@anno", anno) };
            using var reader = _db.ExecuteReader(query, parameters);
            while (reader.Read())
            {
                list.Add(new Iscrizione
                    {
                        Id = reader.GetInt32(0),
                        UtenteId = reader.GetInt32(1),
                        Anno = reader.GetInt32(2),
                        Tipo = reader.GetString(3),
                        DataIscrizione = reader.GetDateTime(4),
                        Utente = new Utente { 
                            Id = reader.GetInt32(5),
                            Nome = reader.GetString(6),
                            Cognome = reader.GetString(7),
                            Email = reader.GetString(8)
                        }
                    }
                );
            }
            return list;
        }
    }
}
