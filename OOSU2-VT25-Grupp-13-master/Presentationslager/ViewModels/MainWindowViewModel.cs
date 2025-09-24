using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.ViewModels
{
    public partial class MainWindowViewModel: ObservableObject
    {
        [RelayCommand]
        private void LoggaIn()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void Registrera()
        {
            var registreraWindow = new RegistreraWindow();
            registreraWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void Avsluta()
        {
            Environment.Exit(0);
        }

        public Action? CloseAction { get; set; }
    }
}
