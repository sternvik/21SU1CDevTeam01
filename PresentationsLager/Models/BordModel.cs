using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace PresentationsLager.Models
{
    public partial class BordModel : ObservableObject
    {
        [ObservableProperty]
        private int bordID;

        [ObservableProperty]
        private int restaurangID;

        [ObservableProperty]
        private string bordkod = string.Empty;

        [ObservableProperty]
        private int antalPlatser;

        [ObservableProperty]
        private string status = "Ledigt";

        [ObservableProperty]
        private RestaurangModel? restaurang;

        [ObservableProperty]
        private ObservableCollection<BokningModel> bokningar = new();

        // Konvertering från entitet till modell
        public static BordModel FromEntity(Bord entity) => new()
        {
            BordID = entity.BordID,
            RestaurangID = entity.RestaurangID,
            Bordkod = entity.Bordkod,
            AntalPlatser = entity.AntalPlatser,
            Status = entity.Status,
            Restaurang = entity.Restaurang != null ? RestaurangModel.FromEntity(entity.Restaurang) : null,
            Bokningar = new ObservableCollection<BokningModel>(
                (entity.Bokningar ?? new List<Bokning>()).Select(BokningModel.FromEntity)
            )
        };

        // Konvertering från modell till entitet
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
