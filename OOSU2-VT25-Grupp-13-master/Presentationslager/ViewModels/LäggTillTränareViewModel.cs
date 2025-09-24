using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Presentationslager.Models; 
using Presentationslager.Personal.Tränarhantering;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Presentationslager.ViewModels
{
    public partial class LäggTillTränareViewModel : ObservableObject
    {
        private readonly SäkerhetsController _säkerhetsController;
        private readonly TränareController _tränareController;

        // Konstruktorn initierar controllers och laddar specialiseringar.
        public LäggTillTränareViewModel()
        {
            _säkerhetsController = new SäkerhetsController();
            _tränareController = new TränareController();

            Specialiseringar = new ObservableCollection<string>(_tränareController.HämtaSpecialisering());
            Specialisering = Specialiseringar.Count > 0 ? Specialiseringar[0] : null;
        }

        // Properties som binder till XAML
        [ObservableProperty]
        private string namn;

        [ObservableProperty]
        private string lösenord;

        [ObservableProperty]
        private string bekräftaLösenord;

        [ObservableProperty]
        private string specialisering;

        public ObservableCollection<string> Specialiseringar { get; }

        public Action? CloseAction { get; set; }

        // Kommandon för knapparna
        [RelayCommand]
        private void LäggTillTränare()
        {
            if (string.IsNullOrWhiteSpace(Namn) || string.IsNullOrWhiteSpace(Lösenord) || string.IsNullOrWhiteSpace(BekräftaLösenord))
            {
                MessageBox.Show("Alla fält måste fyllas i.");
                return;
            }

            if (Lösenord != BekräftaLösenord)
            {
                MessageBox.Show("Lösenorden matchar inte. Kontrollera att du har skrivit samma lösenord i båda fälten.");
                return;
            }

            try
            {
                // Skapa en TränareModel från de bindade fälten
                var tränareModel = new TränareModel
                {
                    Namn = Namn,
                    Lösenord = Lösenord,
                    Specialisering = Specialisering
                };

                // Konvertera TränareModel till Tränare-entitet
                var tränareEntitet = ConvertToEntitet(tränareModel);

                // Skicka tränaren via säkerhetskontrollern
                _säkerhetsController.SkapaTränare(tränareEntitet);
                MessageBox.Show("Tränaren har lagts till!");

                // Navigera tillbaka till TränarhanteringWindow
                var tränarhantering = new TränarhanteringWindow();
                tränarhantering.Show();
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel: {ex.Message}", "Lägg till Tränare-fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metod för att konvertera TränareModel till Tränare entitet
        private Tränare ConvertToEntitet(TränareModel model)
        {
            return new Tränare
            {
                Namn = model.Namn,
                Lösenord = model.Lösenord,
                Specialisering = model.Specialisering
                
            };
        }

        [RelayCommand]
        private void Tillbaka()
        {
            var tränarhantering = new TränarhanteringWindow();
            tränarhantering.Show();
            CloseAction?.Invoke();
        }
    }
}
