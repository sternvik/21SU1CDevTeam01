using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresentationsLager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationsLager.ViewModels
{
    public partial class HamburgerMenuViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }

        private readonly SäkerhetsController _säkerhetsController;

        public HamburgerMenuViewModel()
        {
            _säkerhetsController = new SäkerhetsController();
        }

        [RelayCommand]
        private void MedlemMeny()
        {
            var medlemMenyWindow = new MedlemMenyWindow();
            medlemMenyWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void MinaPass()
        {
            var minaPassWindow = new MinaPassWindow();
            minaPassWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void LoggaUt()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            _säkerhetsController.LoggaUt();
            CloseAction?.Invoke();
        }
    }
}
