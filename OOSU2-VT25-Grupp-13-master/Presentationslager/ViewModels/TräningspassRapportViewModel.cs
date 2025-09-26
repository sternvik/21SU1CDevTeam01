using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Presentationslager.Models;
using Presentationslager.Personal.Rapporter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.ViewModels
{
    public partial class TräningspassRapportViewModel : ObservableObject
    {
        private readonly TräningspassController _träningspassController;
        private readonly TränareController _tränareController;
        private readonly MedlemTräningspassController _medlemTräningspassController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private ObservableCollection<TräningspassModel> träningspassLista = new();

        [ObservableProperty]
        private ObservableCollection<MedlemModel> deltagare = new();

        [ObservableProperty]
        private ObservableCollection<string> specialiseringar = new();

        [ObservableProperty]
        private string valdSpecialisering = "Alla";

        [ObservableProperty]
        private TräningspassModel valdTräningspass;

        [ObservableProperty]
        private TränareModel tränareFörPass;

        [ObservableProperty]
        private string datum;

        [ObservableProperty]
        private string tid;

        public TräningspassRapportViewModel() 
        {
            _träningspassController = new TräningspassController();
            _tränareController = new TränareController();
            _medlemTräningspassController = new MedlemTräningspassController();
            LaddaTräningspass();
            LaddaSpecialiseringar();

        }

        partial void OnValdTräningspassChanged(TräningspassModel value)
        {
            if (value == null)
            {
                Datum = string.Empty;
                Tid = string.Empty;
                Deltagare.Clear();
                TränareFörPass = null;
                return;
            }

            LaddaDeltagare();
            LaddaTränareFörPass();
            Datum = ValdTräningspass.Datum.ToString("yyyy-MM-dd");
        }

        partial void OnValdSpecialiseringChanged(string value)
        {
            FiltreraTräningspass();
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

        private int HämtaAntalDeltagare(int träningspassId)
        {
            return _medlemTräningspassController.HämtaDeltagareFörTräningspass(träningspassId).Count();
        }

        private void LaddaTränareFörPass()
        {
            if (ValdTräningspass == null || ValdTräningspass.TränareID == 0)
            {
                TränareFörPass = null;
                return;
            }

            var hämtadTränare = _träningspassController.HämtaTränareFörTräningspass(ValdTräningspass.TränareID);

            if (hämtadTränare != null)
            {
                TränareFörPass = new TränareModel
                {
                    TränareID = hämtadTränare.TränareID,
                    Namn = hämtadTränare.Namn,
                    Specialisering = hämtadTränare.Specialisering,
                };
            }
            else
            {
                TränareFörPass = null;
            }
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new RapporterWindow().Show();
            CloseAction?.Invoke();
        }
    }
}
