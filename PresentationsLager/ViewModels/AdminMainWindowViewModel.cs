using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using PresentationsLager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                // Logga ut användaren från session
                if (InloggadAnvandare != null)
                {
                    _anvandareController.LoggaUtAnvandare(InloggadAnvandare.AnvandarID);
                }

                var loginWindow = new LoginWindow();
                loginWindow.Show();
                // Stäng denna vy och öppna login igen
                CloseAction?.Invoke();
            }
        }
    }
}
