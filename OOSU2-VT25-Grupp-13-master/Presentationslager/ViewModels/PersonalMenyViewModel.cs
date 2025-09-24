using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentationslager.Personal.Rapporter;
using Presentationslager.Personal.Tränarhantering;
using Presentationslager.Personal.Träningspasshantering;
using Presentationslager.Personal.Utlåningshatering;
using Presentationslager.Personal.Utrustningshantering;
using System;
using System.Windows;

namespace Presentationslager.ViewModels
{
    public partial class PersonalMenyViewModel : ObservableObject
    {
        // Kommando för att öppna Medlemshantering
        [RelayCommand]
        private void OpenMedlemshantering()
        {
            var medlemshanteringWindow = new MedlemshanteringWindow();
            medlemshanteringWindow.Show();
            CloseAction?.Invoke();
        }

        // Kommando för att öppna Utrustningshantering
        [RelayCommand]
        private void OpenUtrustningshantering()
        {
            var utrustningshanteringWindow = new UtrustningshanteringWindow();
            utrustningshanteringWindow.Show();
            CloseAction?.Invoke();
        }

        // Kommando för att öppna Tränarhantering
        [RelayCommand]
        private void OpenTränarhantering()
        {
            var tränarhanteringWindow = new TränarhanteringWindow();
            tränarhanteringWindow.Show();
            CloseAction?.Invoke();
        }

        // Kommando för att öppna Utlåningshantering
        [RelayCommand]
        private void OpenUtlåningshantering()
        {
            var utlåningshanteringWindow = new UtlåningshanteringWindow();
            utlåningshanteringWindow.Show();
            CloseAction?.Invoke();
        }

        // Kommando för att öppna Träningspasshantering
        [RelayCommand]
        private void OpenTräningspasshantering()
        {
            var träningspasshanteringWindow = new TräningspasshanteringWindow();
            träningspasshanteringWindow.Show();
            CloseAction?.Invoke();
        }

        // Kommando för att öppna Rapporter
        [RelayCommand]
        private void OpenRapporter()
        {
            var rapporterWindow = new RapporterWindow();
            rapporterWindow.Show();
            CloseAction?.Invoke();
        }

        // Kommando för att avsluta applikationen
        [RelayCommand]
        private void Avsluta()
        {
            Environment.Exit(0);
        }

        // Action för att stänga nuvarande fönster
        public Action? CloseAction { get; set; }
    }
}
