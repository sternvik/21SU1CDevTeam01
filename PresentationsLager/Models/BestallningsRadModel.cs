using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;

namespace PresentationsLager.Models
{
    public partial class BestallningsRadModel : ObservableObject
    {
        [ObservableProperty] private int bestallningsRadID;
        [ObservableProperty] private int bestallningsID;
        [ObservableProperty] private int menyID;
        [ObservableProperty] private int antal;
        [ObservableProperty] private decimal pris;
        [ObservableProperty] private decimal summa;
        [ObservableProperty] private BestallningModel? bestallning;
        [ObservableProperty] private MenyModel? meny;

        public static BestallningsRadModel FromEntity(BestallningsRad entity) => new()
        {
            BestallningsRadID = entity.BestallningsRadID,
            BestallningsID = entity.BestallningsID,
            MenyID = entity.MenyID,
            Antal = entity.Antal,
            Pris = entity.Pris,
            Summa = entity.Summa,
            Bestallning = entity.Bestallning != null ? BestallningModel.FromEntity(entity.Bestallning) : null,
            Meny = entity.Meny != null ? MenyModel.FromEntity(entity.Meny) : null
        };

        public BestallningsRad ToEntity() => new()
        {
            BestallningsRadID = BestallningsRadID,
            BestallningsID = BestallningsID,
            MenyID = MenyID,
            Antal = Antal,
            Pris = Pris,
            Summa = Summa
        };
    }
}
