using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentationslager.Personal.Rapporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.ViewModels
{
    public partial class RapporterViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }

        public RapporterViewModel() { }

        [RelayCommand]
        private void ÖppnaUtrustningRapport()
        {
            var utrustningsrapportWindow = new UtrustningsrapportWindow();
            utrustningsrapportWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaTräningspassRapport()
        {
            var träningspassrapportWindow = new TräningspassrapportWindow();
            träningspassrapportWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void Tillbaka()
        {
            var personalMenyWindow = new PersonalMenyWindow();
            personalMenyWindow.Show();
            CloseAction?.Invoke();
        }
    }
}
