using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System.Collections.ObjectModel;

namespace PresentationsLager.Models
{
    public class BordModel
    {
        public int BordID { get; set; }
        public int RestaurangID { get; set; }
        public string Bordkod { get; set; } = string.Empty;
        public int AntalPlatser { get; set; }
        public string Status { get; set; } = "Ledigt";
        public RestaurangModel? Restaurang { get; set; }
        public ObservableCollection<BokningModel> Bokningar { get; set; } = new();

        public static BordModel FromEntity(Bord entity) => new()
        {
            BordID = entity.BordID,
            RestaurangID = entity.RestaurangID,
            Bordkod = entity.Bordkod,
            AntalPlatser = entity.AntalPlatser,
            Status = entity.Status,
            Restaurang = entity.Restaurang != null ? RestaurangModel.FromEntity(entity.Restaurang) : null,
            Bokningar = new ObservableCollection<BokningModel>((entity.Bokningar ?? new List<Bokning>()).Select(BokningModel.FromEntity))
        };

        public Bord ToEntity() => new()
        {
            BordID = BordID,
            RestaurangID = RestaurangID,
            Bordkod = Bordkod,
            AntalPlatser = AntalPlatser,
            Status = Status
        };
    }
}
