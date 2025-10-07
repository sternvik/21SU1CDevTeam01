using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;

namespace PresentationsLager.Models
{
    public partial class AnvandareModel : ObservableObject
    {
        [ObservableProperty] private int anvandarID;
        [ObservableProperty] private string anvandarnamn = string.Empty;
        [ObservableProperty] private string losenord = string.Empty;
        [ObservableProperty] private string namn = string.Empty;
        [ObservableProperty] private int? hemmarestaurangID;
        [ObservableProperty] private string roll = string.Empty;
        [ObservableProperty] private bool aktiv;
        [ObservableProperty] private Restaurang? hemmarestaurang;

        public static AnvandareModel FromEntity(Anvandare entity)
        {
            return new AnvandareModel
            {
                AnvandarID = entity.AnvandarID,
                Anvandarnamn = entity.Anvandarnamn,
                Losenord = entity.Losenord,
                Namn = entity.Namn,
                HemmarestaurangID = entity.HemmarestaurangID,
                Roll = entity.Roll,
                Aktiv = entity.Aktiv,
                Hemmarestaurang = entity.Hemmarestaurang
            };
        }

        public Anvandare ToEntity()
        {
            return new Anvandare
            {
                AnvandarID = AnvandarID,
                Anvandarnamn = Anvandarnamn,
                Losenord = Losenord,
                Namn = Namn,
                HemmarestaurangID = HemmarestaurangID,
                Roll = Roll,
                Aktiv = Aktiv,
                Hemmarestaurang = Hemmarestaurang
            };
        }
    }
}
