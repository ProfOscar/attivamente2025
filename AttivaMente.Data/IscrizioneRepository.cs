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

        public List<Iscrizione> GetAll()
        {
            var list = new List<Iscrizione>();
            string query = @"
                SELECT i.Id, i.UtenteId, i.Anno, i.Tipo, i.DataIscrizione,
                        u.Id, u.Nome, u.Cognome, u.Email
                FROM Iscrizioni i
                RIGHT JOIN Utenti u ON i.UtenteId = u.Id
                ORDER BY u.Cognome, u.Nome";
            using var reader = _db.ExecuteReader(query);
            while (reader.Read())
            {
                int id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                int anno = reader.IsDBNull(2) ? DateTime.Now.Year : reader.GetInt32(2);
                string tipo = reader.IsDBNull(3) ? "Nuova" : reader.GetString(3);
                DateTime dataIscrizione = reader.IsDBNull(4) ? DateTime.MaxValue : reader.GetDateTime(4);
                list.Add(new Iscrizione
                {
                    Id = id,
                    UtenteId = reader.GetInt32(5),
                    Anno = anno,
                    Tipo = tipo,
                    DataIscrizione = dataIscrizione,
                    Utente = new Utente
                    {
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

        public bool Exists(int utenteId, int anno)
        {
            string query = "SELECT COUNT(*) FROM Iscrizioni WHERE UtenteId=@u AND Anno=@a";
            var parameters = new[]
            {
                new SqlParameter("@u", utenteId),
                new SqlParameter("@a", anno)
            };
            return (int)_db.ExecuteScalar(query, parameters) > 0;
        }

        public int Renew(int utenteId, int annoSelezionato)
        {
            int annoSuccessivo = annoSelezionato + 1;

            // se già iscritto, esco
            if (Exists(utenteId, annoSuccessivo))
                return 0;

            string sql = @"
                INSERT INTO Iscrizioni (UtenteId, Anno, Tipo, DataIscrizione)
                VALUES (@u, @a, N'Rinnovo', GETDATE())";
            var parameters = new[]
            {
                new SqlParameter("@u", utenteId),
                new SqlParameter("@a", annoSuccessivo)
            };

            return _db.ExecuteNonQuery(sql, parameters);
        }
    }
}
