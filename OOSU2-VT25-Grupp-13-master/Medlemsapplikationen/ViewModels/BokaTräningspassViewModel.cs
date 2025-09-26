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
    public partial class BokaTräningspassViewModel : ObservableObject
    {

        private readonly TräningspassController _träningspassController;
        private readonly TränareController _tränareController;
        private readonly MedlemTräningspassController _medlemTräningspassController;
        private readonly SäkerhetsController _säkerhetsController;

        [ObservableProperty]
        private ObservableCollection<TräningspassModel> träningspassLista = new();

        [ObservableProperty]
        private ObservableCollection<MedlemModel> deltagare = new();

        [ObservableProperty]
        private ObservableCollection<string> specialiseringar = new();

        [ObservableProperty]
        private string valdSpecialisering = "Alla";

        [ObservableProperty]
        private TränareModel valdTränare;

        [ObservableProperty]
        private TräningspassModel valdTräningspass;

        [ObservableProperty]
        private MedlemModel hämtadMedlem;

        public Action? CloseAction { get; set; }

        public BokaTräningspassViewModel() 
        {
            _tränareController = new TränareController();
            _träningspassController = new TräningspassController();
            _medlemTräningspassController = new MedlemTräningspassController();
            _säkerhetsController = new SäkerhetsController();
            KonverteraMedlem();
            LaddaSpecialiseringar();
            LaddaTräningspass();
        }

        partial void OnValdSpecialiseringChanged(string value)
        {
            FiltreraTräningspass();
        }

        partial void OnValdTräningspassChanged(TräningspassModel value)
        {
            LaddaDeltagare();
            LaddaTränareFörPass();
        }

        private int HämtaAntalDeltagare(int träningspassId)
        {
            return _medlemTräningspassController.HämtaDeltagareFörTräningspass(träningspassId).Count();
        }

        private void LaddaDeltagare()
        {
            if (ValdTräningspass == null) return;
            var deltagareLista = _medlemTräningspassController.HämtaDeltagareFörTräningspass(ValdTräningspass.TräningspassID)
                .Select(m => new MedlemModel
                {
                    MedlemID = m.MedlemID,
                    Namn = m.Namn,
                    Epost = m.Epost,
                    Telefonnummer = m.Telefonnummer
                })
                .ToList();

            Deltagare = new ObservableCollection<MedlemModel>(deltagareLista);
        }

        private void LaddaTräningspass()
        {
            var bokadePassIds = _medlemTräningspassController
            .HämtaTräningspassFörMedlem(HämtadMedlem.MedlemID)
            .Select(tp => tp.TräningspassID)
            .ToHashSet();

            var allaTräningspass = _träningspassController.HämtaAllaTräningspass()
                .Where(tp => tp.Datum > DateTime.Now.Date && !bokadePassIds.Contains(tp.TräningspassID))
                .Select(t => new TräningspassModel
                {
                    TräningspassID = t.TräningspassID,
                    Aktivitet = t.Aktivitet,
                    MaxDeltagare = t.MaxDeltagare,
                    Tid = t.Tid.ToString(@"hh\:mm"),
                    Plats = t.Plats,
                    Datum = t.Datum,
                    TränareID = t.TränareID,
                    Beskrivning = t.Beskrivning,
                    DeltagarAntal = HämtaAntalDeltagare(t.TräningspassID),
                })
                .ToList();

            TräningspassLista = new ObservableCollection<TräningspassModel>(allaTräningspass);
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

        private void LaddaSpecialiseringar()
        {
            var specialiseringarLista = _tränareController.HämtaSpecialisering();
            specialiseringarLista.Insert(0, "Alla");
            Specialiseringar = new ObservableCollection<string>(specialiseringarLista);
        }

        private void FiltreraTräningspass()
        {
            var bokadePassIds = _medlemTräningspassController
            .HämtaTräningspassFörMedlem(HämtadMedlem.MedlemID)
            .Select(tp => tp.TräningspassID)
            .ToHashSet();

            var allaTräningspass = _träningspassController.HämtaAllaTräningspass()
                .Where(tp => tp.Datum > DateTime.Now.Date && !bokadePassIds.Contains(tp.TräningspassID))
                .Select(t => new TräningspassModel
                {
                    TräningspassID = t.TräningspassID,
                    Aktivitet = t.Aktivitet,
                    MaxDeltagare = t.MaxDeltagare,
                    Tid = t.Tid.ToString(@"hh\:mm"),
                    Datum = t.Datum,
                    Plats = t.Plats,
                    TränareID = t.TränareID,
                    Beskrivning = t.Beskrivning,
                    DeltagarAntal = HämtaAntalDeltagare(t.TräningspassID)
                });

            if (!string.IsNullOrEmpty(ValdSpecialisering) && ValdSpecialisering != "Alla")
            {
                TräningspassLista = new ObservableCollection<TräningspassModel>(
                    allaTräningspass.Where(t => t.Aktivitet.Equals(ValdSpecialisering, StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                TräningspassLista = new ObservableCollection<TräningspassModel>(allaTräningspass);
            }
        }

        private void LaddaTränareFörPass()
        {
            if (ValdTräningspass == null || ValdTräningspass.TränareID == 0)
            {
                ValdTränare = null;
                return;
            }

            var hämtadTränare = _träningspassController.HämtaTränareFörTräningspass(ValdTräningspass.TränareID);

            if (hämtadTränare != null)
            {
                ValdTränare = new TränareModel
                {
                    TränareID = hämtadTränare.TränareID,
                    Namn = hämtadTränare.Namn,
                    Specialisering = hämtadTränare.Specialisering,
                };
            }
            else
            {
                ValdTränare = null;
            }
        }

        [RelayCommand]
        private void LäggTillDeltagare()
        {
            if (ValdTräningspass == null || HämtadMedlem == null) return;

            int nuvarandeDeltagare = HämtaAntalDeltagare(ValdTräningspass.TräningspassID);

            if (nuvarandeDeltagare >= ValdTräningspass.MaxDeltagare)
            {
                MessageBox.Show("Det går inte att lägga till fler deltagare. Maxgränsen har nåtts!");
                return;
            }

            _medlemTräningspassController.LäggTillMedlemPåTräningspass(
                new Medlem { MedlemID = HämtadMedlem.MedlemID },
                new Träningspass { TräningspassID = ValdTräningspass.TräningspassID });

            ValdTräningspass.DeltagarAntal = HämtaAntalDeltagare(ValdTräningspass.TräningspassID);
            MessageBox.Show("Deltagare tillagd!");
            LaddaDeltagare();
            LaddaTräningspass();
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
            var medlemMenyWindow= new MedlemMenyWindow();
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
            var genomfördaPassWindow =  new GenomfördaPassWindow();
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
