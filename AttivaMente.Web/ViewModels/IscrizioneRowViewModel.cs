namespace AttivaMente.Web.ViewModels
{
    public class IscrizioneRowViewModel
    {
        public int UtenteId { get; set; }
        public string Cognome { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";

        public string? Tipo { get; set; }
        public DateTime? DataIscrizione { get; set; }

        public string Azione { get; set; } = ""; // Cancella | Rinnova | Iscrivi
    }
}