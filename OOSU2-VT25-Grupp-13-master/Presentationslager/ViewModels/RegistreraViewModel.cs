using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presentationslager.ViewModels
{
    public partial class RegistreraViewModel : ObservableObject
    {
        private readonly SäkerhetsController _säkerhetsController;
        private readonly TränareController _tränareController;

        public RegistreraViewModel()
        {
            _säkerhetsController = new SäkerhetsController();
            _tränareController = new TränareController();

            Specialiseringar = new ObservableCollection<string>(_tränareController.HämtaSpecialisering());
            Specialisering = Specialiseringar.Count > 0 ? Specialiseringar[0] : null;
        }

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

        [RelayCommand]
        private void Registrera()
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
                var tränare = new Tränare
                {
                    Namn = Namn,
                    Lösenord = Lösenord,
                    Specialisering = Specialisering
                };

                _säkerhetsController.SkapaTränare(tränare);
                MessageBox.Show("Konto har skapats!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel: {ex.Message}", "Registreringsfel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Tillbaka()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            CloseAction?.Invoke();
        }
    }
}
