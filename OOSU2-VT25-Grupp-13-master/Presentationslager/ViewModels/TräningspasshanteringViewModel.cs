using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentationslager.Personal.Träningspasshantering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentationslager.ViewModels
{
    public partial class TräningspasshanteringViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }

        public TräningspasshanteringViewModel() { }

        [RelayCommand]
        private void ÖppnaSkapaträningspass()
        {
            new SkapaträningspassWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaRedigeraträningspass()
        {
            new RedigeraträningspassWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaTabortträningspass()
        {
            new TabortträningspassWindow().Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaHanteradeltagare()
        {
            new HanteradeltagareWindow().Show();
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
