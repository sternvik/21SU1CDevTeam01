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
    public partial class ÅterlämnaUtlåningViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }
        private readonly MedlemController _medlemController;
        private readonly UtlåningController _utlåningController;
        private readonly UtrustningController _utrustningController;

        [ObservableProperty]
        private ObservableCollection<UtlåningModel> allaUtlåningar = new();

        [ObservableProperty]
        private UtlåningModel valdUtlåning;

        [ObservableProperty]
        private MedlemModel valdMedlem;

        [ObservableProperty]
        private UtrustningModel valdUtrustning;

        [ObservableProperty]
        private DateTime? återlämningsdatum;

        public ÅterlämnaUtlåningViewModel() 
        {
            _utrustningController = new UtrustningController();
            _medlemController = new MedlemController();
            _utlåningController = new UtlåningController();

            LaddaAktivaUtlåningar();
        }

        private void LaddaAktivaUtlåningar()
        {
            AllaUtlåningar.Clear();
            var aktivaUtlåningar = _utlåningController.HämtaAllaAktivaUtlåningar()
                .Where(u => u.Återlämningsdatum == null)
                .Select(u => new UtlåningModel
                {
                    UtlåningID = u.UtlåningID,
                    MedlemID = u.MedlemID,
                    UtrustningID = u.UtrustningID,
                    UtLåningsdatum = u.UtLåningsdatum
                    
                });

            foreach (var utlåning in aktivaUtlåningar)
            {
                AllaUtlåningar.Add(utlåning);
            }
        }

        partial void OnValdUtlåningChanged(UtlåningModel? oldValue, UtlåningModel? newValue)
        {
            if (newValue != null)
            {
                var medlem = _medlemController.HämtaAllaMedlemmar().FirstOrDefault(m => m.MedlemID == newValue.MedlemID);
                var utrustning = _utrustningController.HämtaTillgängligUtrustning().FirstOrDefault(u => u.UtrustningID == newValue.UtrustningID);

                ValdMedlem = medlem != null ? new MedlemModel { MedlemID = medlem.MedlemID, Namn = medlem.Namn } : new MedlemModel { Namn = "Okänd" };
                ValdUtrustning = utrustning != null ? new UtrustningModel { UtrustningID = utrustning.UtrustningID, Namn = utrustning.Namn } : new UtrustningModel { Namn = "Okänd" };
            }
        }

        [RelayCommand]
        private void SparaÅterlämning()
        {
            if (ValdUtlåning == null)
            {
                System.Windows.MessageBox.Show("Vänligen välj en utlåning.");
                return;
            }

            if (Återlämningsdatum == null)
            {
                System.Windows.MessageBox.Show("Vänligen välj ett återlämningsdatum.");
                return;
            }

            var utlåningAttÅterlämna = new Utlåning
            {
                MedlemID = ValdUtlåning.MedlemID,
                UtrustningID = ValdUtlåning.UtrustningID,
                Återlämningsdatum = Återlämningsdatum,

            };

            _utlåningController.RegistreraÅterlämning(utlåningAttÅterlämna);
            System.Windows.MessageBox.Show("Utlåning Återlämnad");

            LaddaAktivaUtlåningar();
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new UtlåningshanteringWindow().Show();
            CloseAction?.Invoke();
        }
    }
}
