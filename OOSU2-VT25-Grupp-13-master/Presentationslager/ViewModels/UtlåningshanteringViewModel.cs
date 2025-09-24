using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentationslager.Personal.Utlåningshatering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.ViewModels
{
    public partial class UtlåningshanteringViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }

        public UtlåningshanteringViewModel() { }

        [RelayCommand]
        private void ÖppnaSkapaUtlåning()
        {
            new SkapautlåningWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaÅterlämnaUtlåning()
        {
            new ÅterlämnautlåningWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaVisaAllaUtlåningar()
        {
            new VisaallautlåningarWindow().Show();
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
