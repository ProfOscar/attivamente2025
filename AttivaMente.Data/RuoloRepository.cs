using AttivaMente.Core.Models;
using Microsoft.Data.SqlClient;

namespace AttivaMente.Data
{
    /// <summary>
    /// Classe per gestire il CRUD (create/read/update/delete) dei ruoli.
    /// </summary>
    public class RuoloRepository
    {
        private Database _db;

        public RuoloRepository(string connStr)
        {
            _db = new Database(connStr);
        }

        public List<Ruolo> GetAll()
        {
            var ruoli = new List<Ruolo>();
            string query = "SELECT Id, Nome FROM Ruoli";

            using var reader = _db.ExecuteReader(query);
            while (reader.Read())
            {
                var ruolo = new Ruolo { 
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1)
                };
                ruoli.Add(ruolo);
            }

            return ruoli;
        }
        
        public Ruolo? GetById(int id) {
            string query = "SELECT Id, Nome FROM Ruoli WHERE Id = @idPlaceholder";
            var parameters = new[] { new SqlParameter("@idPlaceholder", id) };
            using var reader = _db.ExecuteReader(query, parameters);
            if (reader.Read())
            {
                var ruolo = new Ruolo
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1)
                };
                return ruolo;
            }
            return null;
        }

        public int Add(Ruolo ruolo) {
            string sql = "INSERT INTO Ruoli(Nome) VALUES(@nomePlaceholder)";
            var parameters = new[] { new SqlParameter("@nomePlaceholder", ruolo.Nome) };
            return _db.ExecuteNonQuery(sql, parameters);
        }

        public int Update(string nomeRuolo, int idRuolo) {
            string sql = "UPDATE Ruoli SET Nome = @nomePlaceholder WHERE Id = @idPlaceholder";
            var parameters = new[] {
                new SqlParameter("@idPlaceholder", idRuolo),
                new SqlParameter("@nomePlaceholder", nomeRuolo) 
            };
            return _db.ExecuteNonQuery(sql, parameters);
        }
        
        public int Delete(int idRuolo) {
            string sql = "DELETE FROM Ruoli WHERE Id = @idPlaceholder";
            var parameters = new[] { new SqlParameter("@idPlaceholder", idRuolo) };
            return _db.ExecuteNonQuery(sql, parameters);
        }
    }
}
