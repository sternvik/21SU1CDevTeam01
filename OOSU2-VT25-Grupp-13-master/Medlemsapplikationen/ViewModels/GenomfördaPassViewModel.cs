using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Medlemsapplikationen.Models;
using Medlemsapplikationen.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Medlemsapplikationen.ViewModels
{
    public partial class GenomfördaPassViewModel : ObservableObject
    {
        private readonly TräningspassController _träningspassController;
        private readonly TränareController _tränareController;
        private readonly MedlemTräningspassController _medlemTräningspassController;
        private readonly SäkerhetsController _säkerhetsController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private ObservableCollection<TräningspassModel> träningspassLista = new();

        [ObservableProperty]
        private ObservableCollection<string> specialiseringar = new();

        [ObservableProperty]
        private string valdSpecialisering = "Alla";

        [ObservableProperty]
        private MedlemModel hämtadMedlem;

        public GenomfördaPassViewModel() 
        {
            _tränareController = new TränareController();
            _träningspassController = new TräningspassController();
            _medlemTräningspassController = new MedlemTräningspassController();
            _säkerhetsController = new SäkerhetsController();
            KonverteraMedlem();
            LaddaSpecialiseringar();
            LaddaTräningspass();
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

            var genomförda = medlemTräningspass
                .Where(tp => tp.Datum <= nu)
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

            TräningspassLista = new ObservableCollection<TräningspassModel>(genomförda);
        }

        private void FiltreraTräningspass()
        {
            if (HämtadMedlem == null || HämtadMedlem.MedlemID == 0) return;

            int medlemID = HämtadMedlem.MedlemID;
            var nu = DateTime.Now.Date;

            var medlemTräningspass = _medlemTräningspassController
                .HämtaTräningspassFörMedlem(medlemID);

            var genomförda = medlemTräningspass
                .Where(tp => tp.Datum <= nu)
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

            if (string.IsNullOrEmpty(ValdSpecialisering) || ValdSpecialisering == "Alla")
            {
                TräningspassLista = new ObservableCollection<TräningspassModel>(genomförda);
            }
            else
            {
                TräningspassLista = new ObservableCollection<TräningspassModel>(
                    genomförda.Where(tp => tp.Aktivitet.Equals(ValdSpecialisering, StringComparison.OrdinalIgnoreCase))
                );
            }
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
