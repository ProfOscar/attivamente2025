﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using AttivaMente.Core.Models;

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
        
        public Ruolo GetById(int id) {
            string query = $"SELECT Id, Nome FROM Ruoli WHERE Id={id}";
            using var reader = _db.ExecuteReader(query);
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

        // public void Add(Ruolo ruolo) { }
        // public void Update(Ruolo ruolo) { }
        // public void Delete(int id) { }
    }
}
