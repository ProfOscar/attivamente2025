﻿using AttivaMente.Core.Models;
using Microsoft.Data.SqlClient;

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
                utenti.Add(new Utente
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Cognome = reader.GetString(2),
                    Email = reader.GetString(3),
                    PasswordHash = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    // PasswordHash = Convert.ToString(reader["PasswordHash"]) ?? "",
                    RuoloId = reader.GetInt32(5),
                    Ruolo = new Ruolo
                    {
                        Id = reader.GetInt32(6),
                        Nome = reader.GetString(7)
                    }
                });
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
                return new Utente
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Cognome = reader.GetString(2),
                    Email = reader.GetString(3),
                    PasswordHash = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    RuoloId = reader.GetInt32(5),
                    Ruolo = new Ruolo
                    {
                        Id = reader.GetInt32(6),
                        Nome = reader.GetString(7)
                    }
                };
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
    }
}
