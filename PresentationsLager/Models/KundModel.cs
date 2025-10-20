using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System.Collections.ObjectModel;

namespace PresentationsLager.Models
{
    public partial class KundModel : ObservableObject
    {
        [ObservableProperty]
        private int kundID;

        [ObservableProperty]
        private string namn = string.Empty;

        [ObservableProperty]
        private string telefon = string.Empty;

        [ObservableProperty]
        private string? email;

        [ObservableProperty]
        private int lojalitetsPoang;

        [ObservableProperty]
        private string lojalitetsNiva = "Brons";

        [ObservableProperty]
        private int? regionID;

        [ObservableProperty]
        private int? hemmarestaurangID;

        [ObservableProperty]
        private RegionModel? region;

        [ObservableProperty]
        private RestaurangModel? hemmarestaurang;

        // Konvertering från entitet till modell
        public static KundModel FromEntity(Kund entity) => new()
        {
            KundID = entity.KundID,
            Namn = entity.Namn,
            Telefon = entity.Telefon ?? string.Empty,
            Email = entity.Email,
            LojalitetsPoang = entity.LojalitetsPoang,
            LojalitetsNiva = entity.LojalitetsNiva,
            RegionID = entity.RegionID,
            HemmarestaurangID = entity.HemmarestaurangID,
            Region = entity.Region != null ? RegionModel.FromEntity(entity.Region) : null,
            Hemmarestaurang = entity.Hemmarestaurang != null ? RestaurangModel.FromEntity(entity.Hemmarestaurang) : null
        };

        // Konvertering från modell till entitet
        public Kund ToEntity() => new()
        {
            KundID = KundID,
            Namn = Namn,
            Telefon = Telefon,
            Email = Email,
            LojalitetsPoang = LojalitetsPoang,
            LojalitetsNiva = LojalitetsNiva,
            RegionID = RegionID,
            HemmarestaurangID = HemmarestaurangID
        };
    }
}
