using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System.Collections.ObjectModel;
using System.Linq;

namespace PresentationsLager.Models
{
    public partial class RegionModel : ObservableObject
    {
        [ObservableProperty] private int regionID;
        [ObservableProperty] private string regionnamn = string.Empty;
        [ObservableProperty] private int antalRestauranger;

        [ObservableProperty] private ObservableCollection<RestaurangModel> restauranger = new();
        [ObservableProperty] private ObservableCollection<KundModel> kunder = new();

        public static RegionModel FromEntity(Region entity) => new()
        {
            RegionID = entity.RegionID,
            Regionnamn = entity.Regionnamn,
            AntalRestauranger = entity.AntalRestauranger,
            Restauranger = new ObservableCollection<RestaurangModel>((entity.Restauranger ?? new List<Restaurang>()).Select(RestaurangModel.FromEntity)),
            Kunder = new ObservableCollection<KundModel>((entity.Kunder ?? new List<Kund>()).Select(KundModel.FromEntity))
        };

        public Region ToEntity() => new()
        {
            RegionID = RegionID,
            Regionnamn = Regionnamn,
            AntalRestauranger = AntalRestauranger
        };
    }
}
