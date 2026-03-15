namespace AttivaMente.Web.ViewModels
{
    public class IscrizioniIndexViewModel
    {
        public int SelectedYear { get; set; }
        public bool SoloIscritti { get; set; }

        public string Ricerca { get; set; } = "";

        public List<int> Years { get; set; } = new();
        public List<IscrizioneRowViewModel> Rows { get; set; } = new();
    }
}