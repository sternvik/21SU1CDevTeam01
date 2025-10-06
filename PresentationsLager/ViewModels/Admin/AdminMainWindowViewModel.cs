using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using PresentationsLager.Views;
using PresentationsLager.Views.Admin;
using System;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class AdminMainWindowViewModel : ObservableObject
    {
        private readonly AnvandareController _anvandareController;

        [ObservableProperty]
        private Anvandare? inloggadAnvandare;

        public Action? CloseAction { get; set; }

        public AdminMainWindowViewModel()
        {
            _anvandareController = new AnvandareController();
        }

        public void Initialize(Anvandare anvandare)
        {
            InloggadAnvandare = anvandare;
        }

        [RelayCommand]
        private void Stäng()
        {
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void LoggaUt()
        {
            var result = MessageBox.Show($"Vill du logga ut {InloggadAnvandare?.Namn}?", "Logga ut",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (InloggadAnvandare != null)
                    _anvandareController.LoggaUtAnvandare(InloggadAnvandare.AnvandarID);

                var loginWindow = new LoginWindow();
                loginWindow.Show();
                CloseAction?.Invoke();
            }
        }

        [RelayCommand]
        private void ÖppnaMenyhantering()
        {
            var menyWindow = new MenyHanteringWindow();
            menyWindow.ShowDialog();
        }

        [RelayCommand]
        private void HanteraAnvandare()
        {
            var anvWindow = new AnvandareHanteringWindow();
            anvWindow.ShowDialog();
        }

        [RelayCommand]
        private void HanteraKunder()
        {
            var kunderWindow = new KunderHanteringWindow();
            kunderWindow.ShowDialog();
        }
    }
}
