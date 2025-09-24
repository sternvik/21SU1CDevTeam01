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
using System.Windows;

namespace Presentationslager.ViewModels
{
    public partial class SkapaUtlåningViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }
        private readonly MedlemController _medlemController;
        private readonly UtlåningController _utlåningController;
        private readonly UtrustningController _utrustningController;

        [ObservableProperty]
        private ObservableCollection<MedlemModel> allaMedlemmar = new();

        [ObservableProperty]
        private ObservableCollection<UtrustningModel> allUtrustning = new();

        [ObservableProperty]
        private MedlemModel valdMedlem;

        [ObservableProperty]
        private UtrustningModel valdUtrustning;

        [ObservableProperty]
        private DateTime utlåningsdatum;

        [ObservableProperty]
        private DateTime? återlämningsdatum;

        public SkapaUtlåningViewModel() 
        {
            _utrustningController = new UtrustningController();
            _medlemController = new MedlemController();
            _utlåningController = new UtlåningController();

            LaddaMedlemmar();
            LaddaTillgängligUtrustning();
        }

        private void LaddaMedlemmar()
        {
            var medlemmar = _medlemController.HämtaAllaMedlemmar().Select(m => new MedlemModel
            {
                MedlemID = m.MedlemID,
                Namn = m.Namn,
                Telefonnummer = m.Telefonnummer,
                Födelse = m.Födelse,
                Epost = m.Epost,
                Betalstatus = m.Betalstatus,
                Lösenord = m.Lösenord,
                Poäng = m.Poäng,
                Kalorier = m.Kalorier,
            }).ToList();
            AllaMedlemmar = new ObservableCollection<MedlemModel>(medlemmar);
        }

        private void LaddaTillgängligUtrustning()
        {
            var utrustning = _utrustningController.HämtaTillgängligUtrustning().Select(u => new UtrustningModel
            {
                UtrustningID = u.UtrustningID,
                Namn = u.Namn,
                Kategori = u.Kategori,
                Skick = u.Skick,
                Tillgängliga = u.Tillgängliga,
            }).ToList();
            AllUtrustning = new ObservableCollection<UtrustningModel>(utrustning);
        }

        [RelayCommand]
        private void SparaLån()
        {
            if (ValdMedlem == null)
            {
                MessageBox.Show("Vänligen välj en medlem.");
                return;
            }
            if (ValdUtrustning == null)
            {
                MessageBox.Show("Vänligen välj en utrustning.");
                return;
            }
            if (Utlåningsdatum == null)
            {
                MessageBox.Show("Vänligen välj ett utlåningsdatum.");
                return;
            }

            try
            {
                var utlåning = new Utlåning
                {
                    MedlemID = ValdMedlem.MedlemID,
                    UtrustningID = ValdUtrustning.UtrustningID,
                    UtLåningsdatum = Utlåningsdatum,
                    Återlämningsdatum = Återlämningsdatum
                };

                _utlåningController.RegistreraUtlåning(utlåning);
                MessageBox.Show($"Utrustningen '{ValdUtrustning.Namn}' har lånats ut till {ValdMedlem.Namn}.");

                LaddaTillgängligUtrustning();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fel");
            }
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new UtlåningshanteringWindow().Show();
            CloseAction?.Invoke();
        }
    }

}

