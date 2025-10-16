using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System;

namespace PresentationsLager.Models
{
    public partial class RestaurangMenyModel : ObservableObject
    {
        [ObservableProperty] private int restaurangMenyID;
        [ObservableProperty] private int restaurangID;
        [ObservableProperty] private int menyID;
        [ObservableProperty] private decimal? lokalPris;
        [ObservableProperty] private bool tillganglig = true;

        [ObservableProperty] private Restaurang? restaurang;
        [ObservableProperty] private Meny? meny;

        // Mappning från RestaurangMeny (affärslogik) till RestaurangMenyModel
        public static RestaurangMenyModel FromEntity(RestaurangMeny entity) => new()
        {
            RestaurangMenyID = entity.RestaurangMenyID,
            RestaurangID = entity.RestaurangID,
            MenyID = entity.MenyID,
            LokalPris = entity.LokalPris,
            Tillganglig = entity.Tillganglig,
            Restaurang = entity.Restaurang,
            Meny = entity.Meny
        };

        // Mappning från RestaurangMenyModel till RestaurangMeny (affärslogik)
        public RestaurangMeny ToEntity() => new()
        {
            RestaurangMenyID = RestaurangMenyID,
            RestaurangID = RestaurangID,
            MenyID = MenyID,
            LokalPris = LokalPris,
            Tillganglig = Tillganglig,
            Restaurang = Restaurang,
            Meny = Meny
        };
    }
}
