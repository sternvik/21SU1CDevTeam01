using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresentationsLager.Models;
using PresentationsLager.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class ServitorMainWindowViewModel : ObservableObject
    {
        private readonly AnvandareController _anvandareController;
        private readonly RestaurangController _restaurangController;

        [ObservableProperty]
        private AnvandareModel? inloggadAnvandare;

        [ObservableProperty]
        private ObservableCollection<RestaurangModel> tillgangligaRestauranger = new();

        [ObservableProperty]
        private RestaurangModel? valdRestaurang;

        // Håll koll på senast valda kund för enklare arbetsflöde
        public KundModel? SenastValdaKund { get; set; }

        public Action? CloseAction { get; set; }

        public ServitorMainWindowViewModel()
        {
            _anvandareController = new AnvandareController();
            _restaurangController = new RestaurangController();
        }

        public void Initialize(AnvandareModel anvandare)
        {
            InloggadAnvandare = anvandare;

            // Ladda alla tillgängliga restauranger
            var restauranger = _restaurangController.HamtaAllaRestauranger();
            TillgangligaRestauranger.Clear();
            foreach (var restaurang in restauranger)
            {
                TillgangligaRestauranger.Add(RestaurangModel.FromEntity(restaurang));
            }

            // Sätt hemmarestaurang som vald (default vid inloggning)
            if (anvandare.HemmarestaurangID.HasValue)
            {
                ValdRestaurang = TillgangligaRestauranger.FirstOrDefault(r => r.RestaurangID == anvandare.HemmarestaurangID.Value);
            }
        }

        [RelayCommand]
        private void ÖppnaBokningar()
        {
            try
            {
                if (InloggadAnvandare != null && ValdRestaurang != null)
                {
                    var nyBokningWindow = new NyBokningWindow(InloggadAnvandare, ValdRestaurang.RestaurangID, SenastValdaKund);
                    var result = nyBokningWindow.ShowDialog();

                    // Uppdatera senast valda kund om en ny kund valdes i bokningsfönstret
                    if (nyBokningWindow.DataContext is NyBokningWindowViewModel viewModel && viewModel.ValdKund != null)
                    {
                        SenastValdaKund = viewModel.ValdKund;
                    }
                }
                else if (ValdRestaurang == null)
                {
                    MessageBox.Show("Ingen restaurang vald", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
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
            try
            {
                if (InloggadAnvandare != null)
                {
                    // Öppna beställningsfönster - kund väljs alltid inuti fönstret
                    if (ValdRestaurang == null)
                    {
                        MessageBox.Show("Ingen restaurang vald", "Fel",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var bestallningsWindow = new BestallningsWindow(InloggadAnvandare, ValdRestaurang.RestaurangID);
                    bestallningsWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Ingen användare inloggad", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid öppning av beställningssystem: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            try
            {
                if (ValdRestaurang == null)
                {
                    MessageBox.Show("Ingen restaurang vald", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var menyWindow = new Views.MenyWindow(ValdRestaurang.RestaurangID);
                menyWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid öppning av meny: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void LoggaUt()
        {
            var result = MessageBox.Show($"Vill du logga ut {InloggadAnvandare?.Namn}?", "Logga ut",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (InloggadAnvandare != null)
                {
                    _anvandareController.LoggaUtAnvandare(InloggadAnvandare.AnvandarID);
                }

                var loginWindow = new LoginWindow();
                loginWindow.Show();
                CloseAction?.Invoke();
            }
        }
    }
}