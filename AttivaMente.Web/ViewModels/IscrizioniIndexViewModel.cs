namespace AttivaMente.Web.ViewModels
{
    public class IscrizioniIndexViewModel
    {
        public List<int> Years { get; set; } = new();
        public int SelectedYear { get; set; }
        public bool IsSoloIscritti { get; set; }

        public List<IscrizioneRowViewModel> Rows { get; set; } = new();
    }
}
