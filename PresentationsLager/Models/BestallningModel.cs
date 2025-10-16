using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PresentationsLager.Models
{
    public partial class BestallningModel : ObservableObject
    {
        [ObservableProperty] private int bestallningsID;
        [ObservableProperty] private int? bokningsID;
        [ObservableProperty] private int kundID;
        [ObservableProperty] private int restaurangID;
        [ObservableProperty] private int anvandarID;
        [ObservableProperty] private string bestallningsTyp = string.Empty;
        [ObservableProperty] private string? utkorare;
        [ObservableProperty] private decimal totalSumma;
        [ObservableProperty] private bool betald;
        [ObservableProperty] private int poangTilldelas;
        [ObservableProperty] private DateTime datum;
        [ObservableProperty] private TimeSpan tid;

        [ObservableProperty] private Bokning? bokning;
        [ObservableProperty] private Kund? kund;
        [ObservableProperty] private Restaurang? restaurang;
        [ObservableProperty] private Anvandare? anvandareBeh;

        [ObservableProperty] private ObservableCollection<BestallningsRadModel> bestallningsRader = new();
        [ObservableProperty] private ObservableCollection<TransaktionModel> transaktioner = new();

        public static BestallningModel FromEntity(Bestallning entity) => new()
        {
            BestallningsID = entity.BestallningsID,
            BokningsID = entity.BokningsID,
            KundID = entity.KundID,
            RestaurangID = entity.RestaurangID,
            AnvandarID = entity.AnvandarID,
            BestallningsTyp = entity.BestallningsTyp,
            Utkorare = entity.Utkorare,
            TotalSumma = entity.TotalSumma,
            Betald = entity.Betald,
            PoangTilldelas = entity.PoangTilldelas,
            Datum = entity.Datum,
            Tid = entity.Tid,
            Bokning = entity.Bokning,
            Kund = entity.Kund,
            Restaurang = entity.Restaurang,
            AnvandareBeh = entity.AnvandareBeh,
            BestallningsRader = new ObservableCollection<BestallningsRadModel>(
                entity.BestallningsRader.Select(BestallningsRadModel.FromEntity)
            ),
            Transaktioner = new ObservableCollection<TransaktionModel>(
                entity.Transaktioner.Select(TransaktionModel.FromEntity)
            )
        };

        public Bestallning ToEntity() => new()
        {
            BestallningsID = BestallningsID,
            BokningsID = BokningsID,
            KundID = KundID,
            RestaurangID = RestaurangID,
            AnvandarID = AnvandarID,
            BestallningsTyp = BestallningsTyp,
            Utkorare = Utkorare,
            TotalSumma = TotalSumma,
            Betald = Betald,
            PoangTilldelas = PoangTilldelas,
            Datum = Datum,
            Tid = Tid,
            Bokning = Bokning,
            Kund = Kund,
            Restaurang = Restaurang,
            AnvandareBeh = AnvandareBeh,
            BestallningsRader = BestallningsRader.Select(br => br.ToEntity()).ToList(),
            Transaktioner = Transaktioner.Select(t => t.ToEntity()).ToList()
        };
    }
}
