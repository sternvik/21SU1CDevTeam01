using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Medlemsapplikationen.Models;
using Medlemsapplikationen.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medlemsapplikationen.ViewModels
{
    public partial class RabattViewModel : ObservableObject
    {
        private readonly SäkerhetsController _säkerhetsController;
        private readonly MedlemTräningspassController _medlemTräningspassController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private int antalTräningspass;

        [ObservableProperty]
        private MedlemModel hämtadMedlem;
        public RabattViewModel() 
        {
            _säkerhetsController = new SäkerhetsController();
            _medlemTräningspassController = new MedlemTräningspassController();
            KonverteraMedlem();
            HämtaAntalPassFörMedlem();
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

        private void HämtaAntalPassFörMedlem()
        {
            var nu = DateTime.Now.Date;

            var genomfördaPass = _medlemTräningspassController
                .HämtaTräningspassFörMedlem(HämtadMedlem.MedlemID)
                .Where(tp => tp.Datum <= nu)
                .ToList();

            AntalTräningspass = genomfördaPass.Count;

            UppdateraPoäng(AntalTräningspass);
        }

        private void UppdateraPoäng(int antalGenomfördaPass)
        {
            HämtadMedlem.Poäng = antalGenomfördaPass * 100;
            OnPropertyChanged(nameof(HämtadMedlem.Poäng));
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
