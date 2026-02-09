using System.ComponentModel.DataAnnotations;

namespace AttivaMente.Core.Models
{
    public class Utente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        public string Cognome { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email non valida")]
        public string Email { get; set; } = string.Empty;
        
        public string? PasswordHash { get; set; }
        
        public int RuoloId { get; set; }
        public Ruolo? Ruolo { get; set; }

        public string? Telefono { get; set; }

        public string? Indirizzo { get; set; }
        public string? Citta { get; set; }
        public string? Provincia { get; set; }
        public string? CAP { get; set; }

        public override string ToString()
        {
            string retVal = $"{Id}: {Nome} {Cognome} - {Email}";
            if (Ruolo != null)
                retVal += $" - {Ruolo.Nome}";
            return retVal;
        }
    }
}