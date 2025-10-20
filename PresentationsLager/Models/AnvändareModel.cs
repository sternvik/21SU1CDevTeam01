using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System.Collections.ObjectModel;

namespace PresentationsLager.Models
{
    public partial class AnvandareModel : ObservableObject
    {
        [ObservableProperty]
        private int anvandarID;

        [ObservableProperty]
        private string anvandarnamn = string.Empty;

        [ObservableProperty]
        private string losenord = string.Empty;

        [ObservableProperty]
        private string namn = string.Empty;

        [ObservableProperty]
        private int? hemmarestaurangID;

        [ObservableProperty]
        private string roll = string.Empty;

        [ObservableProperty]
        private bool aktiv;

        [ObservableProperty]
        private RestaurangModel? hemmarestaurang;

        [ObservableProperty]
        private ObservableCollection<BestallningModel> bestallningar = new();

        [ObservableProperty]
        private ObservableCollection<BokningModel> bokningar = new();

        // Konvertering från entitet till modell
        public static AnvandareModel FromEntity(Anvandare entity) => new()
        {
            AnvandarID = entity.AnvandarID,
            Anvandarnamn = entity.Anvandarnamn,
            Losenord = entity.Losenord,
            Namn = entity.Namn,
            HemmarestaurangID = entity.HemmarestaurangID,
            Roll = entity.Roll,
            Aktiv = entity.Aktiv,
            Hemmarestaurang = entity.Hemmarestaurang != null
                ? RestaurangModel.FromEntity(entity.Hemmarestaurang)
                : null,
            Bestallningar = new ObservableCollection<BestallningModel>(
                (entity.Bestallningar ?? new List<Bestallning>()).Select(BestallningModel.FromEntity)
            ),
            Bokningar = new ObservableCollection<BokningModel>(
                (entity.Bokningar ?? new List<Bokning>()).Select(BokningModel.FromEntity)
            )
        };

        // Konvertering från modell till entitet
        public Anvandare ToEntity() => new()
        {
            AnvandarID = AnvandarID,
            Anvandarnamn = Anvandarnamn,
            Losenord = Losenord,
            Namn = Namn,
            HemmarestaurangID = HemmarestaurangID,
            Roll = Roll,
            Aktiv = Aktiv
        };
    }
}
