using CommunityToolkit.Mvvm.ComponentModel;

namespace PresentationsLager.ViewModels
{
    /// <summary>
    /// Meny item för visning i menyer och beställningar
    /// </summary>
    public partial class MenyItemViewModel : ObservableObject
    {
        public int MenyID { get; set; }
        public string Rattnamn { get; set; } = string.Empty;
        public string Beskrivning { get; set; } = string.Empty;
        public decimal Pris { get; set; }
        public string Kategori { get; set; } = string.Empty;
    }

    /// <summary>
    /// Beställningsrad för varukorg
    /// </summary>
    public partial class BestallningsRadViewModel : ObservableObject
    {
        public int MenyID { get; set; }
        public string Rattnamn { get; set; } = string.Empty;
        public decimal Pris { get; set; }
        public string Kategori { get; set; } = string.Empty;

        [ObservableProperty]
        private int antal = 1;

        public decimal Totalpris => Pris * antal;
    }
}
