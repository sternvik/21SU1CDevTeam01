using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;

namespace PresentationsLager.Models
{
    public partial class RestaurangMenyModel : ObservableObject
    {
        [ObservableProperty] private int restaurangMenyID;
        [ObservableProperty] private int restaurangID;
        [ObservableProperty] private int menyID;
        [ObservableProperty] private decimal? lokalPris;
        [ObservableProperty] private bool tillganglig = true;

        [ObservableProperty] private RestaurangModel? restaurang;
        [ObservableProperty] private MenyModel? meny;

        // Mappning från RestaurangMeny (affärslogik) till RestaurangMenyModel
        public static RestaurangMenyModel FromEntity(RestaurangMeny entity) => new()
        {
            RestaurangMenyID = entity.RestaurangMenyID,
            RestaurangID = entity.RestaurangID,
            MenyID = entity.MenyID,
            LokalPris = entity.LokalPris,
            Tillganglig = entity.Tillganglig,
            Restaurang = entity.Restaurang != null ? RestaurangModel.FromEntity(entity.Restaurang) : null,
            Meny = entity.Meny != null ? MenyModel.FromEntity(entity.Meny) : null
        };

        // Mappning från RestaurangMenyModel till RestaurangMeny (affärslogik)
        public RestaurangMeny ToEntity() => new()
        {
            RestaurangMenyID = RestaurangMenyID,
            RestaurangID = RestaurangID,
            MenyID = MenyID,
            LokalPris = LokalPris,
            Tillganglig = Tillganglig
        };
    }
}
