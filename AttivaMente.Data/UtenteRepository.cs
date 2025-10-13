using AttivaMente.Core.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AttivaMente.Data
{
    public class UtenteRepository
    {
        private readonly Database _db;

        public UtenteRepository(string connStr)
        {
            _db = new Database(connStr);
        }

        public List<Utente> GetAll()
        {
            var utenti = new List<Utente>();
            string query = @"SELECT u.Id, u.Nome, u.Cognome, u.Email, u.PasswordHash, u.RuoloId, r.Id AS Ruolo_Id, r.Nome AS RuoloNome
                FROM Utenti u INNER JOIN Ruoli r ON u.RuoloId = r.Id";

            using var reader = _db.ExecuteReader(query);
            while (reader.Read())
            {
                Utente utente = MapUtente(reader);
                utenti.Add(utente);
            }
            
            return utenti;
        }

        public List<Utente> Search(string searchTerm)
        {
            var utenti = new List<Utente>();
            var pattern = $"%{searchTerm}%";
			string query = @"SELECT u.Id, u.Nome, u.Cognome, u.Email, u.PasswordHash, u.RuoloId, r.Id AS Ruolo_Id, r.Nome AS RuoloNome
                FROM Utenti u INNER JOIN Ruoli r ON u.RuoloId = r.Id WHERE u.Nome LIKE @pattern OR u.Cognome LIKE @pattern OR u.Email LIKE @pattern";
			var parameters = new[] { new SqlParameter("@pattern", pattern) };

			using var reader = _db.ExecuteReader(query, parameters);
			while (reader.Read())
			{
				utenti.Add(MapUtente(reader));
			}

			return utenti;
        }

        public Utente? GetById(int id)
        {
            string query = @"SELECT u.Id, u.Nome, u.Cognome, u.Email, u.PasswordHash, u.RuoloId, r.Id AS Ruolo_Id, r.Nome AS RuoloNome
                FROM Utenti u INNER JOIN Ruoli r ON u.RuoloId = r.Id
                WHERE u.Id = @idPlaceholder";
            var parameters = new[] { new SqlParameter("@idPlaceholder", id) };
            using var reader = _db.ExecuteReader(query, parameters);
            if (reader.Read())
            {
                return MapUtente(reader);
            }
            return null;
        }

        public int Add(Utente utente)
        {
            string sql = "INSERT INTO Utenti (Nome, Cognome, Email, PasswordHash, RuoloId) " +
                                $"VALUES (@nomePlaceholder, @cognPlaceholder, @emailPlaceholder, @pwHashPlaceholder, @ruoloIdPlaceholder)";
            var parameters = new[] {
                new SqlParameter("@nomePlaceholder", utente.Nome),
                new SqlParameter("@cognPlaceholder", utente.Cognome),
                new SqlParameter("@emailPlaceholder", utente.Email),
                new SqlParameter("@pwHashPlaceholder", utente.PasswordHash),
                new SqlParameter("@ruoloIdPlaceholder", utente.RuoloId)
            };
            return _db.ExecuteNonQuery(sql, parameters);
        }

        public int Update(Utente utente)
        {
            // Se password assente, recupero il suo attuale valore
            if (string.IsNullOrWhiteSpace(utente.PasswordHash))
            {
                var esistente = GetById(utente.Id);
                if (esistente == null) return 0;
                utente.PasswordHash = esistente.PasswordHash;
            }
            string sql = "UPDATE Utenti SET " +
                                "Nome = @nomePlaceholder, Cognome = @cognPlaceholder, " +
                                "Email = @emailPlaceholder, PasswordHash = @pwHashPlaceholder, RuoloId = @ruoloIdPlaceholder " +
                                "WHERE Id = @idPlaceholder";
            var parameters = new[] {
                new SqlParameter("@idPlaceholder", utente.Id),
                new SqlParameter("@nomePlaceholder", utente.Nome),
                new SqlParameter("@cognPlaceholder", utente.Cognome),
                new SqlParameter("@emailPlaceholder", utente.Email),
                new SqlParameter("@pwHashPlaceholder", utente.PasswordHash),
                new SqlParameter("@ruoloIdPlaceholder", utente.RuoloId)
            };
            return _db.ExecuteNonQuery(sql, parameters);
        }

        public int Delete(int id)
        {
            string sql = "DELETE FROM Utenti WHERE Id = @idPlaceholder";
            var parameters = new[] { new SqlParameter("@idPlaceholder", id) };
            return _db.ExecuteNonQuery(sql, parameters);
        }

        private static Utente MapUtente(IDataRecord record)
        {
            return new Utente
            {
                Id = record.GetInt32(0),
                Nome = record.GetString(1),
                Cognome = record.GetString(2),
                Email = record.GetString(3),
                PasswordHash = record.IsDBNull(4) ? "" : record.GetString(4),
                // PasswordHash = Convert.ToString(record["PasswordHash"]) ?? "",
                RuoloId = record.GetInt32(5),
                Ruolo = new Ruolo
                {
                    Id = record.GetInt32(6),
                    Nome = record.GetString(7)
                }
            };
		}
    }
}
