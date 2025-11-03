using AttivaMente.Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
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
                FROM Utenti u INNER JOIN Ruoli r ON u.RuoloId = r.Id ORDER BY u.Cognome, u.Nome";

            using var reader = _db.ExecuteReader(query);
            while (reader.Read())
            {
                Utente utente = MapUtente(reader);
                utenti.Add(utente);
            }
            
            return utenti;
        }

        public List<Utente> Search(
            string searchTerm, 
            int? ruoloFilter, 
            string? orderBy, 
            string? direction,
            int page = 1,
            int pageSize = 10)
        {
            var utenti = new List<Utente>();
            var pattern = $"%{searchTerm}%";
			string query = @"SELECT u.Id, u.Nome, u.Cognome, u.Email, u.PasswordHash, u.RuoloId, r.Id AS Ruolo_Id, r.Nome AS RuoloNome
                FROM Utenti u INNER JOIN Ruoli r ON u.RuoloId = r.Id WHERE (1=1)";

			var parameters = new List<SqlParameter>();
            
            // --- ricerca ---
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += " AND (u.Nome LIKE @pattern OR u.Cognome LIKE @pattern OR u.Email LIKE @pattern)";
                parameters.Add(new SqlParameter("@pattern", pattern));
            }

            // --- filtri ---
            if (ruoloFilter > 0)
            {
                query += " AND u.RuoloId = @ruoloFilter";
                parameters.Add(new SqlParameter("@ruoloFilter", ruoloFilter));
            }

            // --- ordinamento ---
            string colonna = orderBy?.ToLower() switch
            {
                "id" => "u.Id",
                "nome" => "u.Nome",
                "cognome" => "u.Cognome",
                "email" => "u.Email",
                "ruolo" => "r.Nome",
                _ => "u.Cognome"
            };
            string direzione = (direction?.ToLower() == "desc") ? "DESC" : "ASC";
            query += $" ORDER BY {colonna} {direzione}";
			if (colonna != "u.Cognome") query += ", u.Cognome ASC";
			if (colonna != "u.Nome") query += ", u.Nome ASC";

            // --- paginazione ---
            int offset = (page - 1) * pageSize;
            query += $" OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

			using var reader = _db.ExecuteReader(query, parameters.ToArray());
			while (reader.Read()) utenti.Add(MapUtente(reader));

			return utenti;
        }

        public int Count(string? searchTerm, int? ruoloFilter)
        {
            string query = @"SELECT COUNT(*) FROM Utenti WHERE (1=1)";
            var parameters = new List<SqlParameter>();

            // --- ricerca ---
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += " AND (Nome LIKE @pattern OR Cognome LIKE @pattern OR Email LIKE @pattern)";
                parameters.Add(new SqlParameter("@pattern", $"%{searchTerm}%"));
            }

            // --- filtri ---
            if (ruoloFilter.HasValue && ruoloFilter > 0)
            {
                query += " AND RuoloId = @ruoloFilter";
                parameters.Add(new SqlParameter("@ruoloFilter", ruoloFilter));
            }

            object result = _db.ExecuteScalar(query, parameters.ToArray());
            int count = Convert.ToInt32(result);

            return count;
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
