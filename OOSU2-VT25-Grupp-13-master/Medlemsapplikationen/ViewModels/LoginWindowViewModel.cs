using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Medlemsapplikationen.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Medlemsapplikationen.ViewModels
{
    public partial class LoginWindowViewModel : ObservableObject
    {
        private readonly SäkerhetsController _säkerhetsController = new SäkerhetsController();


        [ObservableProperty]
        private string användarnamn = string.Empty;

        [ObservableProperty]
        private string lösenord = string.Empty;

        public Action? CloseAction { get; set; }

        [RelayCommand]
        private void LoggaIn()
        {
            Console.WriteLine($"Användarnamn: {Användarnamn}");
            Console.WriteLine($"Lösenord: {Lösenord}");

            bool ärAutentiserad = _säkerhetsController.LoggaInMedlem(Användarnamn, Lösenord);

            if (ärAutentiserad)
            {
                MessageBox.Show("Inloggning lyckades!");
                var medlemMenyWindow = new MedlemMenyWindow();
                medlemMenyWindow.Show();
                CloseAction?.Invoke();
            }
            else
            {
                MessageBox.Show("Fel användarnamn eller lösenord!");
            }
        }

        [RelayCommand]
        private void Tillbaka()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            CloseAction?.Invoke();
        }
    }
}
