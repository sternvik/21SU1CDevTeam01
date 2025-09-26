using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentationslager.Personal.Tränarhantering;
using System;

namespace Presentationslager.ViewModels
{
    public partial class TränarhanteringViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }


        [RelayCommand]
        private void LäggTillTränare()
        {

            new LäggtilltränareWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void UppdateraTränare()
        {

            new UppdateratränareWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void TaBortTränare()
        {

            new TaborttränareWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void VisaAllaTränare()
        {

            new VisaAllaTränareWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void Tillbaka()
        {

            new PersonalMenyWindow().Show();
            CloseAction?.Invoke();
        }
    }
}