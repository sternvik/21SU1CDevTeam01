using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Presentationslager.Models;
using Presentationslager.Personal.Träningspasshantering;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Presentationslager.ViewModels
{
    public partial class HanteradeltagareViewModel : ObservableObject
    {
        private readonly TräningspassController _träningspassController;
        private readonly TränareController _tränareController;
        private readonly MedlemTräningspassController _medlemTräningspassController;

        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private TräningspassModel valdTräningspass;

        [ObservableProperty]
        private MedlemModel valdMedlem;

        [ObservableProperty]
        private MedlemModel valdDeltagare;

        [ObservableProperty]
        private string valdSpecialisering = "Alla";

        [ObservableProperty]
        private ObservableCollection<string> specialiseringar = new();

        [ObservableProperty]
        private ObservableCollection<TräningspassModel> träningspassLista = new();

        [ObservableProperty]
        private ObservableCollection<MedlemModel> deltagare = new();

        [ObservableProperty]
        private ObservableCollection<MedlemModel> tillgängligaMedlemmar = new();

        public HanteradeltagareViewModel()
        {
            _tränareController = new TränareController();
            _träningspassController = new TräningspassController();
            _medlemTräningspassController = new MedlemTräningspassController();

            LaddaSpecialiseringar();
            LaddaTräningspass();

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ValdSpecialisering))
                {
                    FiltreraTräningspass();
                }
                else if (e.PropertyName == nameof(ValdTräningspass) && ValdTräningspass != null)
                {
                    LaddaTillgängligaMedlemmar();
                    LaddaDeltagare();
                }
            };
        }

        private void LaddaTräningspass()
        {
            var träningspassLista = _träningspassController.HämtaAllaTräningspass()
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
                    DeltagarAntal = HämtaAntalDeltagare(t.TräningspassID),
                })
                .ToList();

            TräningspassLista = new ObservableCollection<TräningspassModel>(träningspassLista);
        }

        private void LaddaSpecialiseringar()
        {
            var specialiseringarLista = _tränareController.HämtaSpecialisering();
            specialiseringarLista.Insert(0, "Alla");
            Specialiseringar = new ObservableCollection<string>(specialiseringarLista);
        }

        private void FiltreraTräningspass()
        {
            var allaTräningspass = _träningspassController.HämtaAllaTräningspass()
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

        private void LaddaTillgängligaMedlemmar()
        {
            if (ValdTräningspass == null) return;
            var tillgängligaMedlemmarLista = _medlemTräningspassController.HämtaTillgängligaMedlemmarFörTräningspass(ValdTräningspass.TräningspassID)
                .Select(m => new MedlemModel
                {
                    MedlemID = m.MedlemID,
                    Namn = m.Namn,
                    Epost = m.Epost,
                    Telefonnummer = m.Telefonnummer
                })
                .ToList();

            TillgängligaMedlemmar = new ObservableCollection<MedlemModel>(tillgängligaMedlemmarLista);
        }

        [RelayCommand]
        private void LäggTillDeltagare()
        {
            if (ValdTräningspass == null || ValdMedlem == null) return;

            int nuvarandeDeltagare = HämtaAntalDeltagare(ValdTräningspass.TräningspassID);

            if (nuvarandeDeltagare >= ValdTräningspass.MaxDeltagare)
            {
                MessageBox.Show("Det går inte att lägga till fler deltagare. Maxgränsen har nåtts!");
                return;
            }

            _medlemTräningspassController.LäggTillMedlemPåTräningspass(
                new Medlem { MedlemID = ValdMedlem.MedlemID },
                new Träningspass { TräningspassID = ValdTräningspass.TräningspassID });

            ValdTräningspass.DeltagarAntal = HämtaAntalDeltagare(ValdTräningspass.TräningspassID);
            MessageBox.Show("Deltagare tillagd!");
            LaddaDeltagare();
            LaddaTillgängligaMedlemmar();
            LaddaTräningspass();
        }

        [RelayCommand]
        private void TaBortDeltagare()
        {
            if (ValdTräningspass == null || ValdDeltagare == null) return;

            _medlemTräningspassController.TaBortMedlemFrånPass(
                new Medlem { MedlemID = ValdDeltagare.MedlemID },
                new Träningspass { TräningspassID = ValdTräningspass.TräningspassID });

            ValdTräningspass.DeltagarAntal = HämtaAntalDeltagare(ValdTräningspass.TräningspassID);
            MessageBox.Show("Deltagare borttagen!");
            LaddaDeltagare();
            LaddaTillgängligaMedlemmar();
            LaddaTräningspass();
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new TräningspasshanteringWindow().Show();
            CloseAction?.Invoke();
        }
    }
}
