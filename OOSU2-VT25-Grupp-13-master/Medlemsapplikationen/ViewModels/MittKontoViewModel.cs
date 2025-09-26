using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Medlemsapplikationen.Models;
using Medlemsapplikationen.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Medlemsapplikationen.ViewModels
{
    public partial class MittKontoViewModel : ObservableObject
    {
        private readonly SäkerhetsController _säkerhetsController;
        private readonly MedlemController _medlemController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private MedlemModel hämtadMedlem;

        [ObservableProperty]
        private string bekräftaLösenord;
        public MittKontoViewModel() 
        {
            _medlemController = new MedlemController();
            _säkerhetsController = new SäkerhetsController();

            KonverteraMedlem();
        }

        private void KonverteraMedlem()
        {
            var medlem = _säkerhetsController.GetLoggedInMember();

            HämtadMedlem = new MedlemModel
            {
                MedlemID = medlem.MedlemID,
                Namn = medlem.Namn,
                Epost = medlem.Epost,
                Telefonnummer = medlem.Telefonnummer,
                Födelse = medlem.Födelse,
                Lösenord = medlem.Lösenord,
                Poäng = medlem.Poäng,
                Kalorier = medlem.Kalorier,
                Betalstatus = medlem.Betalstatus
            };
        }

        [RelayCommand]
        private void UppdateraMedlem()
        {
            if (HämtadMedlem.Lösenord != BekräftaLösenord)
            {
                MessageBox.Show("Lösenorden matchar inte. Kontrollera att du har skrivit samma lösenord i båda fälten.");
                return;
            }

            var medlem = _säkerhetsController.GetLoggedInMember();

            medlem.Namn = HämtadMedlem.Namn;
            medlem.Födelse = HämtadMedlem.Födelse;
            medlem.Telefonnummer = HämtadMedlem.Telefonnummer;
            medlem.Epost = HämtadMedlem.Epost;
            medlem.Lösenord = HämtadMedlem.Lösenord;
            medlem.Kalorier = HämtadMedlem.Kalorier;
            medlem.Poäng = HämtadMedlem.Poäng;
            medlem.Betalstatus = HämtadMedlem.Betalstatus;

            _medlemController.UppdateraMedlem(medlem);

            InloggadMedlemSingleton.GetInstance().SetInloggadMedlem(medlem);

            MessageBox.Show("Konto uppgifter sparade!");
        }


        [RelayCommand]
        private void Tillbaka()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            _säkerhetsController.LoggaUt();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaBokaTräninspass()
        {
            var bokaTräningspassWindow = new BokaTräningspassWindow();
            bokaTräningspassWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaMedlemMeny()
        {
            var medlemMenyWindow = new MedlemMenyWindow();
            medlemMenyWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaMittKonto()
        {
            var mittKontoWindow = new MittKontoWindow();
            mittKontoWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaKommandePass()
        {
            var kommandePassWindow = new KommandePassWindow();
            kommandePassWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaGenomfördaPass()
        {
            var genomfördaPassWindow = new GenomfördaPassWindow();
            genomfördaPassWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaAllaMedlemmar()
        {
            var allaMedlemmarWindow = new AllaMedlemarWindow();
            allaMedlemmarWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void ÖppnaRabattNivå()
        {
            var rabattWindow = new RabattWindow();
            rabattWindow.Show();
            CloseAction?.Invoke();
        }

    }
}
