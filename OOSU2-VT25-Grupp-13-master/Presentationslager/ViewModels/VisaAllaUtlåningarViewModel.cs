using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Presentationslager.Models;
using Presentationslager.Personal.Utlåningshatering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.ViewModels
{
    public partial class VisaAllaUtlåningarViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }
        private readonly MedlemController _medlemController;
        private readonly UtlåningController _utlåningController;
        private readonly UtrustningController _utrustningController;

        [ObservableProperty]
        private ObservableCollection<UtlåningModel> allaUtlåningar = new();

        [ObservableProperty]
        private ObservableCollection<string> statusAlternativ = new() { "Alla", "Aktiva", "Historik" };

        [ObservableProperty]
        private string valdStatus = "Alla";

        [ObservableProperty]
        private UtlåningModel? valdUtlåning;

        [ObservableProperty]
        private MedlemModel? valdMedlem;

        [ObservableProperty]
        private UtrustningModel? valdUtrustning;

        [ObservableProperty]
        private string? utlåningsdatum;

        [ObservableProperty]
        private string? återlämningsdatum;

        public VisaAllaUtlåningarViewModel() 
        {
            _utrustningController = new UtrustningController();
            _medlemController = new MedlemController();
            _utlåningController = new UtlåningController();
            LaddaUtlåningar();
        }

        private void LaddaUtlåningar()
        {
            var alla = HämtaOchKonverteraUtlåningar(_utlåningController.HämtaALlaUtlåningar());
            var aktiva = HämtaOchKonverteraUtlåningar(_utlåningController.HämtaAllaAktivaUtlåningar());
            var historik = HämtaOchKonverteraUtlåningar(_utlåningController.HämtaArkiveradeUtlånignar());

            UppdateraUtlåningslista(alla, aktiva, historik);
        }

        private List<UtlåningModel> HämtaOchKonverteraUtlåningar(IEnumerable<Utlåning> utlåningar)
        {
            return utlåningar.Select(u => new UtlåningModel
            {
                UtlåningID = u.UtlåningID,
                MedlemID = u.MedlemID,
                UtrustningID = u.UtrustningID,
                UtLåningsdatum = u.UtLåningsdatum,
                Återlämningsdatum = u.Återlämningsdatum
            }).ToList();
        }

        private void UppdateraUtlåningslista(List<UtlåningModel> alla, List<UtlåningModel> aktiva, List<UtlåningModel> historik)
        {
            AllaUtlåningar.Clear();

            var listaAttLäggaTill = ValdStatus switch
            {
                "Aktiva" => aktiva,
                "Historik" => historik,
                _ => alla
            };

            foreach (var utlåning in listaAttLäggaTill)
            {
                AllaUtlåningar.Add(utlåning);
            }
        }

        partial void OnValdStatusChanged(string value)
        {
            LaddaUtlåningar();
        }

        partial void OnValdUtlåningChanged(UtlåningModel? value)
        {
            if (value == null) return;

            var medlem = _medlemController.HämtaAllaMedlemmar().FirstOrDefault(m => m.MedlemID == value.MedlemID);
            var utrustning = _utrustningController.HämtaTillgängligUtrustning().FirstOrDefault(u => u.UtrustningID == value.UtrustningID);

            ValdMedlem = medlem != null ? new MedlemModel { MedlemID = medlem.MedlemID, Namn = medlem.Namn } : new MedlemModel { Namn = "Okänd" };
            ValdUtrustning = utrustning != null ? new UtrustningModel { UtrustningID = utrustning.UtrustningID, Namn = utrustning.Namn } : new UtrustningModel { Namn = "Okänd" };
            Utlåningsdatum = value.UtLåningsdatum.ToString("yyyy-MM-dd");
            Återlämningsdatum = value.Återlämningsdatum?.ToString("yyyy-MM-dd") ?? "Ej återlämnad";
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new UtlåningshanteringWindow().Show();
            CloseAction?.Invoke();
        }
    }
}
