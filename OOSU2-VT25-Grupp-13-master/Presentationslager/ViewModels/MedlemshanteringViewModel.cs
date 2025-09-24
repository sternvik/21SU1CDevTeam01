using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentationslager.Personal.Medlemshantering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.ViewModels
{
    public partial class MedlemshanteringViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }

        public MedlemshanteringViewModel()
        {}

        [RelayCommand]
        private void ÖppnaLäggTillMedlem()
        {
            new LäggtillmedlemWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaUppdateraMedlem()
        {
            new UppdateramedlemWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaTaBortMedlem()
        {
            new TabortmedlemWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaVisaMedlemStatus()
        {
            new VisamedlemstatusWindow().Show();
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
