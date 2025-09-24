using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Presentationslager.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Presentationslager.ViewModels
{
    public partial class TaBortTränareViewModel : ObservableObject
    {
        private readonly TränareController _tränareController;

        // Använd TränareModel här
        public TaBortTränareViewModel()
        {
            _tränareController = new TränareController();
            TränareList = new ObservableCollection<TränareModel>(HämtaAllaTränare()); // Hämta modellerna istället
        }

        // Lista med TränareModel
        public ObservableCollection<TränareModel> TränareList { get; }

        // Property för vald tränare
        [ObservableProperty]
        private TränareModel selectedTränare;

        // Kommando för att ta bort vald tränare
        [RelayCommand]
        private void TaBortTränare()
        {
            if (SelectedTränare == null)
            {
                MessageBox.Show("Du måste välja en tränare att ta bort!");
                return;
            }

            try
            {
                // Konvertera tillbaka till entitet här för att ta bort från databasen
                var entitet = ConvertToEntitet(SelectedTränare);
                _tränareController.TaBortTränare(entitet);
                MessageBox.Show("Tränare borttagen!");

                // Uppdatera listan efter borttagning
                TränareList.Remove(SelectedTränare);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid borttagning av tränare: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Kommando för att gå tillbaka och öppna tränarhantering
        [RelayCommand]
        private void Tillbaka()
        {
            new Presentationslager.Personal.Tränarhantering.TränarhanteringWindow().Show();
            CloseAction?.Invoke(); // Stänger nuvarande fönster
        }

        // Action för att stänga fönstret via ViewModel
        public Action? CloseAction { get; set; }

        // Metod för att hämta alla tränare som modeller
        private ObservableCollection<TränareModel> HämtaAllaTränare()
        {
            var tränareList = _tränareController.HämtaAllaTränare();
            var modelList = new ObservableCollection<TränareModel>();

            foreach (var tränare in tränareList)
            {
                var model = new TränareModel
                {
                    TränareID = tränare.TränareID,
                    Namn = tränare.Namn,
                    Specialisering = tränare.Specialisering,
                    Lösenord = tränare.Lösenord 
                };

                modelList.Add(model);
            }

            return modelList;
        }

        // Metod för att konvertera TränareModel till Tränare entitet
        private Tränare ConvertToEntitet(TränareModel model)
        {
            return new Tränare
            {
                TränareID = model.TränareID,
                Namn = model.Namn,
                Specialisering = model.Specialisering,
                Lösenord = model.Lösenord // Ta med lösenordet här
            };
        }
    }
}
