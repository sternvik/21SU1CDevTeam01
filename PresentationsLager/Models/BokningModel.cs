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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    namespace PresentationsLager.Models
    {
        public partial class BokningModel : ObservableObject
        {
            [ObservableProperty] private int bokningsID;
            [ObservableProperty] private int kundID;
            [ObservableProperty] private int bordID;
            [ObservableProperty] private int restaurangID;
            [ObservableProperty] private int? anvandarID;
            [ObservableProperty] private DateTime datum;
            [ObservableProperty] private TimeSpan tid;
            [ObservableProperty] private int antalGaster;
            [ObservableProperty] private string? specialinformation;
            [ObservableProperty] private string bokningsTyp = string.Empty;
            [ObservableProperty] private string status = "Bokad";
            [ObservableProperty] private DateTime skapadDatum = DateTime.Now;

            [ObservableProperty] private Kund? kund;
            [ObservableProperty] private Bord? bord;
            [ObservableProperty] private Restaurang? restaurang;
            [ObservableProperty] private Anvandare? anvandare;

            [ObservableProperty] private ObservableCollection<BestallningModel> bestallningar = new();

            public static BokningModel FromEntity(Bokning entity) => new()
            {
                BokningsID = entity.BokningsID,
                KundID = entity.KundID,
                BordID = entity.BordID,
                RestaurangID = entity.RestaurangID,
                AnvandarID = entity.AnvandarID,
                Datum = entity.Datum,
                Tid = entity.Tid,
                AntalGaster = entity.AntalGaster,
                Specialinformation = entity.Specialinformation,
                BokningsTyp = entity.BokningsTyp,
                Status = entity.Status,
                SkapadDatum = entity.SkapadDatum,
                Kund = entity.Kund,
                Bord = entity.Bord,
                Restaurang = entity.Restaurang,
                Anvandare = entity.Anvandare,
                Bestallningar = new ObservableCollection<BestallningModel>(entity.Bestallningar.Select(BestallningModel.FromEntity))
            };

            public Bokning ToEntity() => new()
            {
                BokningsID = BokningsID,
                KundID = KundID,
                BordID = BordID,
                RestaurangID = RestaurangID,
                AnvandarID = AnvandarID,
                Datum = Datum,
                Tid = Tid,
                AntalGaster = AntalGaster,
                Specialinformation = Specialinformation,
                BokningsTyp = BokningsTyp,
                Status = Status,
                SkapadDatum = SkapadDatum,
                Kund = Kund,
                Bord = Bord,
                Restaurang = Restaurang,
                Anvandare = Anvandare,
                Bestallningar = Bestallningar.Select(b => b.ToEntity()).ToList()
            };
        }
    }

}
