using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationsLager.Models
{
    public partial class KundModel : ObservableObject
    {
        [ObservableProperty] private int kundID;
        [ObservableProperty] private string namn = string.Empty;
        [ObservableProperty] private string telefon = string.Empty;
        [ObservableProperty] private string? email;
        [ObservableProperty] private int lojalitetsPoang;
        [ObservableProperty] private string lojalitetsNiva = "Brons";
        [ObservableProperty] private int? regionID;
        [ObservableProperty] private int? hemmarestaurangID;
        [ObservableProperty] private Region? region;
        [ObservableProperty] private Restaurang? hemmarestaurang;

        public static KundModel FromEntity(Kund entity) => new()
        {
            KundID = entity.KundID,
            Namn = entity.Namn,
            Telefon = entity.Telefon,
            Email = entity.Email,
            LojalitetsPoang = entity.LojalitetsPoang,
            LojalitetsNiva = entity.LojalitetsNiva,
            RegionID = entity.RegionID,
            HemmarestaurangID = entity.HemmarestaurangID,
            Region = entity.Region,
            Hemmarestaurang = entity.Hemmarestaurang
        };

        public Kund ToEntity() => new()
        {
            KundID = KundID,
            Namn = Namn,
            Telefon = Telefon,
            Email = Email,
            LojalitetsPoang = LojalitetsPoang,
            LojalitetsNiva = LojalitetsNiva,
            RegionID = Region?.RegionID ?? RegionID,
            HemmarestaurangID = Hemmarestaurang?.RestaurangID ?? HemmarestaurangID,
            Region = Region,
            Hemmarestaurang = Hemmarestaurang
        };
    }
}
