using EntitetsLager;
using System.ComponentModel;

namespace PresentationsLager.Models
{
    public class AnvandareModel : INotifyPropertyChanged
    {
        public int AnvandarID { get; set; }
        public string Anvandarnamn { get; set; } = string.Empty;
        public string Losenord { get; set; } = string.Empty;
        public string Namn { get; set; } = string.Empty;
        public int? HemmarestaurangID { get; set; }
        public string Roll { get; set; } = string.Empty;
        public bool Aktiv { get; set; }
        public RestaurangModel? Hemmarestaurang { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public static AnvandareModel FromEntity(Anvandare entity) => new()
        {
            AnvandarID = entity.AnvandarID,
            Anvandarnamn = entity.Anvandarnamn,
            Losenord = entity.Losenord,
            Namn = entity.Namn,
            HemmarestaurangID = entity.HemmarestaurangID,
            Roll = entity.Roll,
            Aktiv = entity.Aktiv,
            Hemmarestaurang = entity.Hemmarestaurang != null ? RestaurangModel.FromEntity(entity.Hemmarestaurang) : null
        };

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
