using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataLager;
using EntitetsLager;
using PresentationsLager.Views;
using System;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class ServitorMainWindowViewModel : ObservableObject
    {
        private readonly AnvandareController _anvandareController;

        [ObservableProperty]
        private Anvandare? inloggadAnvandare;

        [ObservableProperty]
        private string hemmarestaurangNamn = string.Empty;

        [ObservableProperty]
        private int aktivaBokningar = 0;

        [ObservableProperty]
        private int bestallningarIdag = 0;

        // Håll koll på senast valda kund för enklare arbetsflöde
        public Kund? SenastValdaKund { get; set; }

        public Action? CloseAction { get; set; }

        public ServitorMainWindowViewModel()
        {
            _anvandareController = new AnvandareController();
        }

        public void Initialize(Anvandare anvandare)
        {
            InloggadAnvandare = anvandare;

            // Sätt hemmarestaurang namn (för nu bara visa ID, senare hämta från databas)
            HemmarestaurangNamn = $"Restaurang {anvandare.HemmarestaurangID}";

            // Mock data för status (senare hämta från databas)
            AktivaBokningar = 5;
            BestallningarIdag = 12;
        }

        [RelayCommand]
        private void ÖppnaBokningar()
        {
            try
            {
                if (InloggadAnvandare != null)
                {
                    var nyBokningWindow = new NyBokningWindow(InloggadAnvandare, SenastValdaKund);
                    var result = nyBokningWindow.ShowDialog();

                    // Uppdatera senast valda kund om en ny kund valdes i bokningsfönstret
                    if (nyBokningWindow.DataContext is NyBokningWindowViewModel viewModel && viewModel.ValdKund != null)
                    {
                        SenastValdaKund = viewModel.ValdKund;
                    }
                }
                else
                {
                    MessageBox.Show("Ingen användare inloggad", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid öppning av bokningssystem: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        [RelayCommand]
        private void NyBestallning()
        {
            MessageBox.Show("Öppnar beställningssystem...\n(Kommer att implementeras)", "Beställningar",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void SökKunder()
        {
            try
            {
                var kundSearchWindow = new KundSearchWindow();
                var result = kundSearchWindow.ShowDialog();

                // Spara vald kund för senare användning
                if (kundSearchWindow.DataContext is KundSearchWindowViewModel viewModel && viewModel.ValdKund != null)
                {
                    SenastValdaKund = viewModel.ValdKund;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid öppning av kundsökning: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void VisaMeny()
        {
            MessageBox.Show("Visar dagens meny...\n(Kommer att implementeras)", "Meny",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void Hjälp()
        {
            MessageBox.Show("Hjälp och support\n\nKontakta IT-support för hjälp med systemet.", "Hjälp",
                MessageBoxButton.OK, MessageBoxImage.Information);
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

                // Stäng denna vy och öppna login igen
                CloseAction?.Invoke();

                var loginWindow = new LoginWindow();
                loginWindow.Show();
            }
        }
    }
}