using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System;

namespace PresentationsLager.Models
{
    public partial class SystemloggModel : ObservableObject
    {
        [ObservableProperty] private int loggID;
        [ObservableProperty] private int? anvandarID;
        [ObservableProperty] private string modul = string.Empty;
        [ObservableProperty] private string handelse = string.Empty;
        [ObservableProperty] private DateTime datum;
        [ObservableProperty] private TimeSpan tid;
        [ObservableProperty] private string? ipAdress;

        [ObservableProperty] private AnvandareModel? anvandare;

        public static SystemloggModel FromEntity(Systemlogg entity) => new()
        {
            LoggID = entity.LoggID,
            AnvandarID = entity.AnvandarID,
            Modul = entity.Modul,
            Handelse = entity.Handelse,
            Datum = entity.Datum,
            Tid = entity.Tid,
            IpAdress = entity.IPAdress,
            Anvandare = entity.Anvandare != null ? AnvandareModel.FromEntity(entity.Anvandare) : null
        };

        public Systemlogg ToEntity() => new()
        {
            LoggID = LoggID,
            AnvandarID = AnvandarID,
            Modul = Modul,
            Handelse = Handelse,
            Datum = Datum,
            Tid = Tid,
            IPAdress = IpAdress
        };
    }
}
