using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;

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

        [ObservableProperty] private BestallningModel? bestallning;
        [ObservableProperty] private RestaurangModel? restaurang;
        [ObservableProperty] private AnvandareModel? anvandare;

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
            Bestallning = entity.Bestallning != null ? BestallningModel.FromEntity(entity.Bestallning) : null,
            Restaurang = entity.Restaurang != null ? RestaurangModel.FromEntity(entity.Restaurang) : null,
            Anvandare = entity.Anvandare != null ? AnvandareModel.FromEntity(entity.Anvandare) : null
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
            TotalSumma = TotalSumma
        };
    }
}
