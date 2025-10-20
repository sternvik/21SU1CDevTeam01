using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using System.Windows.Controls;

namespace PresentationsLager.Models
{
    public partial class BokningModel : ObservableObject
    {
        [ObservableProperty]
        private int bokningsID;

        [ObservableProperty]
        private int kundID;

        [ObservableProperty]
        private int bordID;

        [ObservableProperty]
        private int restaurangID;

        [ObservableProperty]
        private int? anvandarID;

        [ObservableProperty]
        private DateTime datum;

        [ObservableProperty]
        private TimeSpan tid;

        [ObservableProperty]
        private int antalGaster;

        [ObservableProperty]
        private string? specialinformation;

        [ObservableProperty]
        private string bokningsTyp = string.Empty;

        [ObservableProperty]
        private string status = "Bokad";

        [ObservableProperty]
        private DateTime skapadDatum = DateTime.Now;

        [ObservableProperty]
        private KundModel? kund;

        [ObservableProperty]
        private BordModel? bord;

        [ObservableProperty]
        private RestaurangModel? restaurang;

        [ObservableProperty]
        private AnvandareModel? anvandare;

        // Konvertering från entitet till modell
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
            Kund = entity.Kund != null ? KundModel.FromEntity(entity.Kund) : null,
            Bord = entity.Bord != null ? BordModel.FromEntity(entity.Bord) : null,
            Restaurang = entity.Restaurang != null ? RestaurangModel.FromEntity(entity.Restaurang) : null,
            Anvandare = entity.Anvandare != null ? AnvandareModel.FromEntity(entity.Anvandare) : null
        };

        // Konvertering från modell till entitet
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
            SkapadDatum = SkapadDatum
        };
    }
}
