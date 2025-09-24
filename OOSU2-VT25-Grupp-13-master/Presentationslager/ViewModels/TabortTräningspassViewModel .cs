using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Presentationslager.Models;
using Presentationslager.Personal.Träningspasshantering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presentationslager.ViewModels
{
    public partial class TabortTräningspassViewModel : ObservableObject
    {
        private readonly TräningspassController _träningspassController;
        private readonly TränareController _tränareController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private TräningspassModel valdTräningspass;

        [ObservableProperty]
        private string valdSpecialisering = "Alla";

        [ObservableProperty]
        private ObservableCollection<string> specialiseringar = new();

        [ObservableProperty]
        private ObservableCollection<TräningspassModel> träningspassLista = new();

        public TabortTräningspassViewModel()
        {
            _träningspassController = new TräningspassController();
            _tränareController = new TränareController();
            LaddaSpecialiseringar();
            LaddaTräningspass();

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ValdSpecialisering))
                {
                    FiltreraTräningspass();
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
                    DeltagarAntal = t.DeltagarAntal
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

        private Träningspass KonverteraTillTräningspass(TräningspassModel träningspassModel)
        {
            return new Träningspass
            {
                TräningspassID = träningspassModel.TräningspassID,
                Aktivitet = träningspassModel.Aktivitet,
                MaxDeltagare = träningspassModel.MaxDeltagare,
                Tid = TimeSpan.Parse(träningspassModel.Tid),
                Datum = träningspassModel.Datum,
                Plats = träningspassModel.Plats,
                TränareID = träningspassModel.TränareID,
                Beskrivning = träningspassModel.Beskrivning
            };
        }

        [RelayCommand]
        private void TaBortTräningspass()
        {
            if (ValdTräningspass == null)
            {
                MessageBox.Show($"Vänligen välj ett träningspass.");
                return;
            }

            var träningspass = KonverteraTillTräningspass(ValdTräningspass);
            _träningspassController.TaBortTräningspass(träningspass);
            MessageBox.Show("Träningspass borttaget.");
            LaddaTräningspass(); // Ladda om träningspassen
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new TräningspasshanteringWindow().Show();
            CloseAction?.Invoke();
        }

    }
}
