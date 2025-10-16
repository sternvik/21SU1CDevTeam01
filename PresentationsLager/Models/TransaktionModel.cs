using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationsLager.Models
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using EntitetsLager;
    using System;

    namespace PresentationsLager.Models
    {
        public partial class TransaktionModel : ObservableObject
        {
            [ObservableProperty] private int transaktionsID;
            [ObservableProperty] private int bestallningsID;
            [ObservableProperty] private int restaurangID;
            [ObservableProperty] private int anvandarID;
            [ObservableProperty] private DateTime datum;
            [ObservableProperty] private decimal matSumma;
            [ObservableProperty] private decimal alkoholSumma;
            [ObservableProperty] private decimal moms;
            [ObservableProperty] private decimal totalSumma;

            [ObservableProperty] private Bestallning? bestallning;
            [ObservableProperty] private Restaurang? restaurang;
            [ObservableProperty] private Anvandare? anvandare;

            public static TransaktionModel FromEntity(Transaktion entity) => new()
            {
                TransaktionsID = entity.TransaktionsID,
                BestallningsID = entity.BestallningsID,
                RestaurangID = entity.RestaurangID,
                AnvandarID = entity.AnvandarID,
                Datum = entity.Datum,
                MatSumma = entity.MatSumma,
                AlkoholSumma = entity.AlkoholSumma,
                Moms = entity.Moms,
                TotalSumma = entity.TotalSumma,
                Bestallning = entity.Bestallning,
                Restaurang = entity.Restaurang,
                Anvandare = entity.Anvandare
            };

            public Transaktion ToEntity() => new()
            {
                TransaktionsID = TransaktionsID,
                BestallningsID = BestallningsID,
                RestaurangID = RestaurangID,
                AnvandarID = AnvandarID,
                Datum = Datum,
                MatSumma = MatSumma,
                AlkoholSumma = AlkoholSumma,
                Moms = Moms,
                TotalSumma = TotalSumma,
                Bestallning = Bestallning,
                Restaurang = Restaurang,
                Anvandare = Anvandare
            };
        }
    }

}
