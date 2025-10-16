using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System;

namespace PresentationsLager.Models
{
    public partial class LojalitetsTransaktionModel : ObservableObject
    {
        [ObservableProperty] private int lojalitetsTransaktionsID;
        [ObservableProperty] private int kundID;
        [ObservableProperty] private int? bestallningsID;
        [ObservableProperty] private int poangTillagda;
        [ObservableProperty] private int poangAnvanda;
        [ObservableProperty] private int poangSaldo;
        [ObservableProperty] private DateTime datum;

        [ObservableProperty] private Kund? kund;
        [ObservableProperty] private Bestallning? bestallning;

        public static LojalitetsTransaktionModel FromEntity(LojalitetsTransaktion entity) => new()
        {
            LojalitetsTransaktionsID = entity.LojalitetsTransaktionsID,
            KundID = entity.KundID,
            BestallningsID = entity.BestallningsID,
            PoangTillagda = entity.PoangTillagda,
            PoangAnvanda = entity.PoangAnvanda,
            PoangSaldo = entity.PoangSaldo,
            Datum = entity.Datum,
            Kund = entity.Kund,
            Bestallning = entity.Bestallning
        };

        public LojalitetsTransaktion ToEntity() => new()
        {
            LojalitetsTransaktionsID = LojalitetsTransaktionsID,
            KundID = KundID,
            BestallningsID = BestallningsID,
            PoangTillagda = PoangTillagda,
            PoangAnvanda = PoangAnvanda,
            PoangSaldo = PoangSaldo,
            Datum = Datum,
            Kund = Kund,
            Bestallning = Bestallning
        };
    }
}
