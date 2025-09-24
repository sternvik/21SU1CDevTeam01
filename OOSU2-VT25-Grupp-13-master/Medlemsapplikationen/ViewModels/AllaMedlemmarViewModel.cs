using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Medlemsapplikationen.Models;
using Medlemsapplikationen.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medlemsapplikationen.ViewModels
{
    public partial class AllaMedlemmarViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }
        private readonly MedlemController _medlemController;
        private readonly SäkerhetsController _säkerhetsController;
        private readonly MedlemTräningspassController _medlemTräningspassController;

        [ObservableProperty]
        private ObservableCollection<MedlemModel> medlemmarLista = new();

        [ObservableProperty]
        private MedlemModel valdMedlem;

        [ObservableProperty]
        private int antalTräningspass;
        public AllaMedlemmarViewModel()
        {
            _medlemController = new MedlemController();
            _säkerhetsController = new SäkerhetsController();
            _medlemTräningspassController = new MedlemTräningspassController();
            LaddaMedlemmar();
        }

        private void LaddaMedlemmar()
        {
            var medlemmarLista = _medlemController.HämtaAllaMedlemmar()
                .Select(t => new MedlemModel
                {
                    MedlemID = t.MedlemID,
                    Namn = t.Namn,
                    Poäng = t.Poäng,
                    Kalorier = t.Kalorier,
                })
                .ToList();

            MedlemmarLista = new ObservableCollection<MedlemModel>(medlemmarLista);
        }

        partial void OnValdMedlemChanged(MedlemModel value)
        {
            HämtaAntalPassFörMedlem();
        }

        private void HämtaAntalPassFörMedlem()
        {
            var nu = DateTime.Now.Date;

            var genomfördaPass = _medlemTräningspassController
                .HämtaTräningspassFörMedlem(ValdMedlem.MedlemID)
                .Where(tp => tp.Datum <= nu)
                .ToList();

            AntalTräningspass = genomfördaPass.Count;

            UppdateraPoäng(AntalTräningspass);
            UppdateraKalorier(AntalTräningspass);

            OnPropertyChanged(nameof(AntalTräningspass));
        }

        private void UppdateraPoäng(int antalGenomfördaPass)
        {
            ValdMedlem.Poäng = antalGenomfördaPass * 100;
            OnPropertyChanged(nameof(ValdMedlem.Poäng));
        }

        private void UppdateraKalorier(int antalGenomfördaPass)
        {
            ValdMedlem.Kalorier = antalGenomfördaPass * 500;
            OnPropertyChanged(nameof(ValdMedlem.Kalorier));
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
        private void ÖppnaRabattNivå()
        {
            var rabattWindow = new RabattWindow();
            rabattWindow.Show();
            CloseAction?.Invoke();
        }
    }
}
