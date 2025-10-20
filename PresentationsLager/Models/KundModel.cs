using EntitetsLager;
using System;
using System.ComponentModel;

namespace PresentationsLager.Models
{
    public class KundModel : INotifyPropertyChanged
    {
        public int KundID { get; set; }
        public string Namn { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int LojalitetsPoang { get; set; }
        public string LojalitetsNiva { get; set; } = "Brons";
        public int? RegionID { get; set; }
        public int? HemmarestaurangID { get; set; }
        public RegionModel? Region { get; set; }
        public RestaurangModel? Hemmarestaurang { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

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
