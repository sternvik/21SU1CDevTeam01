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
    public partial class LoginWindowViewModel : ObservableObject
    {
        private readonly AnvandareController _anvandareController;

        [ObservableProperty]
        private string anvandarnamn = string.Empty;

        [ObservableProperty]
        private string losenord = string.Empty;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        public Action? CloseAction { get; set; }
        public Action<Anvandare>? NavigateToMainMenu { get; set; }

        public LoginWindowViewModel()
        {
            _anvandareController = new AnvandareController();
        }

        [RelayCommand]
        private void LoggaIn()
        {
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Anvandarnamn) || string.IsNullOrWhiteSpace(Losenord))
            {
                StatusMessage = "Ange både användarnamn och lösenord";
                return;
            }

            try
            {
                bool isAuthenticated = _anvandareController.AutentiseraAnvandare(Anvandarnamn, Losenord);

                if (isAuthenticated)
                {
                    var anvandare = _anvandareController.HamtaInloggadAnvandare(Anvandarnamn);
                    if (anvandare != null)
                    {
                        StatusMessage = $"Välkommen {anvandare.Namn}!";

                        // Navigera baserat på användarens roll
                        NavigateBasedOnRole(anvandare);
                        CloseAction?.Invoke();
                    }
                }
                else
                {
                    StatusMessage = "Felaktigt användarnamn eller lösenord";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fel vid inloggning: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Avsluta()
        {
            Environment.Exit(0);
        }

        // För testing - visa användarnamn/lösenord
        [RelayCommand]
        private void VisaTestAnvandare()
        {
            StatusMessage = "Test: servitor1/password123, admin1/admin123";
        }

        private void NavigateBasedOnRole(Anvandare anvandare)
        {
            try
            {
                switch (anvandare.Roll?.ToLower())
                {
                    case "servitör":
                        var servitorWindow = new ServitorMainWindow(anvandare);
                        servitorWindow.Show();
                        break;

                    case "admin":
                        var adminMainWindow = new AdminMainWindow(anvandare);
                        adminMainWindow.Show();
                        break;

                    case "restaurangchef":
                        MessageBox.Show($"Restaurangchef-vy kommer snart!\nInloggad som: {anvandare.Namn}",
                            "Restaurangchef", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;

                    case "vd":
                        MessageBox.Show($"VD-vy kommer snart!\nInloggad som: {anvandare.Namn}",
                            "VD", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;

                    default:
                        MessageBox.Show($"Okänd roll: {anvandare.Roll}\nKontakta systemadministratör.",
                            "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid navigation: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}