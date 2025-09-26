using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Medlemsapplikationen.Models;
using Medlemsapplikationen.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Medlemsapplikationen.ViewModels
{
    public partial class KommandePassViewmodel: ObservableObject
    {
        private readonly TräningspassController _träningspassController;
        private readonly TränareController _tränareController;
        private readonly MedlemTräningspassController _medlemTräningspassController;
        private readonly SäkerhetsController _säkerhetsController;

        [ObservableProperty]
        private ObservableCollection<TräningspassModel> träningspassLista = new();

        [ObservableProperty]
        private ObservableCollection<string> specialiseringar = new();

        [ObservableProperty]
        private string valdSpecialisering = "Alla";

        [ObservableProperty]
        private TräningspassModel valdTräningspass;

        [ObservableProperty]
        private MedlemModel hämtadMedlem;

        public Action? CloseAction { get; set; }

        public KommandePassViewmodel() 
        {
            _tränareController = new TränareController();
            _träningspassController = new TräningspassController();
            _medlemTräningspassController = new MedlemTräningspassController();
            _säkerhetsController = new SäkerhetsController();
            KonverteraMedlem();
            LaddaTräningspass();
            LaddaSpecialiseringar();
        }

        private void KonverteraMedlem()
        {
            var medlem = _säkerhetsController.GetLoggedInMember();

            HämtadMedlem = new MedlemModel
            {
                MedlemID = medlem.MedlemID,
                Namn = medlem.Namn,
                Epost = medlem.Epost,
                Telefonnummer = medlem.Telefonnummer,
                Födelse = medlem.Födelse,
                Lösenord = medlem.Lösenord,
                Poäng = medlem.Poäng,
                Kalorier = medlem.Kalorier,
                Betalstatus = medlem.Betalstatus
            };
        }

        partial void OnValdSpecialiseringChanged(string value)
        {
            FiltreraTräningspass();
        }

        private int HämtaAntalDeltagare(int träningspassId)
        {
            return _medlemTräningspassController.HämtaDeltagareFörTräningspass(träningspassId).Count();
        }

        private void LaddaSpecialiseringar()
        {
            var specialiseringarLista = _tränareController.HämtaSpecialisering();
            specialiseringarLista.Insert(0, "Alla");
            Specialiseringar = new ObservableCollection<string>(specialiseringarLista);
        }

        private void LaddaTräningspass()
        {
            if (HämtadMedlem == null || HämtadMedlem.MedlemID == 0) return;
            int medlemID = HämtadMedlem.MedlemID;

            var medlemTräningspass = _medlemTräningspassController
                .HämtaTräningspassFörMedlem(medlemID);

            var nu = DateTime.Now.Date;

            var kommande = medlemTräningspass
                .Where(tp => tp.Datum > nu)
                .Select(tp => new TräningspassModel
                {
                    TräningspassID = tp.TräningspassID,
                    Aktivitet = tp.Aktivitet,
                    MaxDeltagare = tp.MaxDeltagare,
                    Tid = tp.Tid.ToString(@"hh\:mm"),
                    Plats = tp.Plats,
                    Datum = tp.Datum,
                    TränareID = tp.TränareID,
                    Beskrivning = tp.Beskrivning,
                    DeltagarAntal = HämtaAntalDeltagare(tp.TräningspassID)
                }).ToList();

            TräningspassLista = new ObservableCollection<TräningspassModel>(kommande);
        }

        private void FiltreraTräningspass()
        {
            if (HämtadMedlem == null || HämtadMedlem.MedlemID == 0) return;
            int medlemID = HämtadMedlem.MedlemID;

            var medlemTräningspass = _medlemTräningspassController
                .HämtaTräningspassFörMedlem(medlemID)
                .Where(tp => tp.Datum > DateTime.Now.Date)
                .Select(tp => new TräningspassModel
                {
                    TräningspassID = tp.TräningspassID,
                    Aktivitet = tp.Aktivitet,
                    MaxDeltagare = tp.MaxDeltagare,
                    Tid = tp.Tid.ToString(@"hh\:mm"),
                    Plats = tp.Plats,
                    Datum = tp.Datum,
                    TränareID = tp.TränareID,
                    Beskrivning = tp.Beskrivning,
                    DeltagarAntal = HämtaAntalDeltagare(tp.TräningspassID)
                });

            if (!string.IsNullOrEmpty(ValdSpecialisering) && ValdSpecialisering != "Alla")
            {
                medlemTräningspass = medlemTräningspass
                    .Where(tp => tp.Aktivitet.Equals(ValdSpecialisering, StringComparison.OrdinalIgnoreCase));
            }

            TräningspassLista = new ObservableCollection<TräningspassModel>(medlemTräningspass);
        }

        [RelayCommand]
        private void AvanmälTräningspass()
        {
            if (ValdTräningspass == null || HämtadMedlem == null) return;

            _medlemTräningspassController.TaBortMedlemFrånPass(
                new Medlem { MedlemID = HämtadMedlem.MedlemID },
                new Träningspass { TräningspassID = ValdTräningspass.TräningspassID });

            MessageBox.Show("Du har avanmält dig från träningspasset.");

            LaddaTräningspass(); // Uppdatera listorna
        }

        [RelayCommand]
        private void Tillbaka()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            _säkerhetsController.LoggaUt();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaBokaTräninspass()
        {
            var bokaTräningspassWindow = new BokaTräningspassWindow();
            bokaTräningspassWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaMedlemMeny()
        {
            var medlemMenyWindow = new MedlemMenyWindow();
            medlemMenyWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaMittKonto()
        {
            var mittKontoWindow = new MittKontoWindow();
            mittKontoWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaKommandePass()
        {
            var kommandePassWindow = new KommandePassWindow();
            kommandePassWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaGenomfördaPass()
        {
            var genomfördaPassWindow = new GenomfördaPassWindow();
            genomfördaPassWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaAllaMedlemmar()
        {
            var allaMedlemmarWindow = new AllaMedlemarWindow();
            allaMedlemmarWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaRabattNivå()
        {
            var rabattWindow = new RabattWindow();
            rabattWindow.Show();
            CloseAction?.Invoke();
        }
    }
}
