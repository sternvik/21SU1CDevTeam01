using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System.Collections.ObjectModel;
using System.Linq;

namespace PresentationsLager.Models
{
    public partial class RestaurangModel : ObservableObject
    {
        [ObservableProperty] private int restaurangID;
        [ObservableProperty] private string restaurangnamn = string.Empty;
        [ObservableProperty] private int regionID;
        [ObservableProperty] private string? adress;
        [ObservableProperty] private string? telefon;
        [ObservableProperty] private string oppettider = "10:30-23:00"; // Default value

        [ObservableProperty] private RegionModel? region;

        [ObservableProperty] private ObservableCollection<BordModel> bord = new();
        [ObservableProperty] private ObservableCollection<BokningModel> bokningar = new();
        [ObservableProperty] private ObservableCollection<BestallningModel> bestallningar = new();
        [ObservableProperty] private ObservableCollection<AnvandareModel> anvandare = new();
        [ObservableProperty] private ObservableCollection<RestaurangMenyModel> restaurangMenyer = new();

        public static RestaurangModel FromEntity(Restaurang entity) => new()
        {
            RestaurangID = entity.RestaurangID,
            Restaurangnamn = entity.Restaurangnamn,
            RegionID = entity.RegionID,
            Adress = entity.Adress,
            Telefon = entity.Telefon,
            Oppettider = entity.Oppettider,
            Region = entity.Region != null ? RegionModel.FromEntity(entity.Region) : null,
            Bord = new ObservableCollection<BordModel>((entity.Bord ?? new List<Bord>()).Select(BordModel.FromEntity)),
            Bokningar = new ObservableCollection<BokningModel>((entity.Bokningar ?? new List<Bokning>()).Select(BokningModel.FromEntity)),
            Bestallningar = new ObservableCollection<BestallningModel>((entity.Bestallningar ?? new List<Bestallning>()).Select(BestallningModel.FromEntity)),
            Anvandare = new ObservableCollection<AnvandareModel>((entity.Anvandare ?? new List<Anvandare>()).Select(AnvandareModel.FromEntity)),
            RestaurangMenyer = new ObservableCollection<RestaurangMenyModel>((entity.RestaurangMenyer ?? new List<RestaurangMeny>()).Select(RestaurangMenyModel.FromEntity))
        };

        public Restaurang ToEntity() => new()
        {
            RestaurangID = RestaurangID,
            Restaurangnamn = Restaurangnamn,
            RegionID = RegionID,
            Adress = Adress,
            Telefon = Telefon,
            Oppettider = Oppettider
        };
    }
}
