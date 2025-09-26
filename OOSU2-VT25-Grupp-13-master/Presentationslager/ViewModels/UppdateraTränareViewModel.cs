using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AffärsLager;
using EntitetsLager;
using Presentationslager.Models; 

namespace Presentationslager.ViewModels
{
    public partial class UppdateraTränareViewModel : ObservableObject
    {
        private readonly TränareController _tränareController;
        public Action? CloseAction { get; set; }

        // Använd TränareModel istället för Tränare entitet här
        [ObservableProperty]
        private ObservableCollection<TränareModel> tränare;

        // Använd TränareModel istället för Tränare entitet
        [ObservableProperty]
        private TränareModel valdTränare;

        [ObservableProperty]
        private string lösenordConfirm;

        [ObservableProperty]
        private ObservableCollection<string> specialiseringar;

        public UppdateraTränareViewModel()
        {
            _tränareController = new TränareController();
            LaddaTränare();
            LaddaSpecialiseringar();
        }

        // Laddar alla tränare och visar dem i en ObservableCollection av TränareModel
        private void LaddaTränare()
        {
            var tränareLista = _tränareController.HämtaAllaTränare().ToList();

            // Konvertera Tränare till TränareModel
            Tränare = new ObservableCollection<TränareModel>(tränareLista.Select(t => new TränareModel
            {
                TränareID = t.TränareID,
                Namn = t.Namn,
                Specialisering = t.Specialisering,
                Lösenord = t.Lösenord 
            }));
        }

        // Ladda specialiseringar för tränare
        private void LaddaSpecialiseringar()
        {
            var specialiseringLista = _tränareController.HämtaSpecialisering();
            Specialiseringar = new ObservableCollection<string>(specialiseringLista);
        }

        [RelayCommand]
        private void Spara()
        {
            if (ValdTränare == null)
            {
                MessageBox.Show("Vänligen välj en tränare att uppdatera.");
                return;
            }

            if (ValdTränare.Lösenord != LösenordConfirm)
            {
                MessageBox.Show("Lösenorden matchar inte.");
                return;
            }

            // Uppdaterar tränaren
            var tränareEntity = new Tränare
            {
                TränareID = ValdTränare.TränareID,
                Namn = ValdTränare.Namn,
                Specialisering = ValdTränare.Specialisering,
                Lösenord = ValdTränare.Lösenord // Ta med lösenordet här
            };

            _tränareController.UppdateraTränare(tränareEntity);
            MessageBox.Show("Tränare uppdaterad!");

            // Ladda om tränare efter uppdatering
            LaddaTränare();
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new Presentationslager.Personal.Tränarhantering.TränarhanteringWindow().Show();
            CloseAction?.Invoke();
        }
    }
}
