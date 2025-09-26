using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentationslager.Personal.Utrustningshantering;
using System;

namespace Presentationslager.ViewModels
{
    public partial class UtrustningshanteringsViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }
        // Kommando för registrering av utrustning
        [RelayCommand]
        private void RegistreraUtrustning()
        {
            new RegistrerautrustningWindow().Show();
            CloseAction?.Invoke();
        }

        // Kommando för att uppdatera utrustning
        [RelayCommand]
        private void UppdateraUtrustning()
        {
            new UppdaterautrustningWindow().Show();
            CloseAction?.Invoke();
        }

        // Kommando för att ta bort utrustning
        [RelayCommand]
        private void TaBortUtrustning()
        {
            new TabortutrustningWindow().Show();
            CloseAction?.Invoke();
        }

        // Kommando för att visa all utrustning
        [RelayCommand]
        private void VisaAllUtrustning()
        {
            new VisaallutrustningWindow().Show();
            CloseAction?.Invoke();
        }

        // Kommando för att gå tillbaka
        [RelayCommand]
        private void Tillbaka()
        {
            new PersonalMenyWindow().Show();
            CloseAction?.Invoke();
        }
    }
}
