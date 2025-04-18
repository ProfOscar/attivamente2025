﻿namespace AttivaMente.Core.Models
{
    public class Utente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RuoloId { get; set; }
        public Ruolo Ruolo { get; set; }

        public override string ToString()
        {
            string retVal = $"{Id}: {Nome} {Cognome} - {Email}";
            if (Ruolo != null)
                retVal += $" - {Ruolo.Nome}";
            return retVal;
        }
    }
}