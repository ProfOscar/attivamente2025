namespace AttivaMente.Core.Models
{
    public class Iscrizione
    {
        public int Id { get; set; }
        public int Anno { get; set; }
        public string Tipo { get; set; } = ""; // "Nuova" | "Rinnovo"
        public DateTime DataIscrizione { get; set; }

        public int UtenteId { get; set; }
        public Utente? Utente { get; set; }
    }
}
