using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager.Entiteter;
using PresentationsLager.Models;
using PresentationsLager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class MedlemMenyViewModel : ObservableObject
    {
        private readonly SäkerhetsController _säkerhetsController;
        private readonly TräningspassController _träningspassController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private int antalTräningspass;

        [ObservableProperty]
        private MedlemModel hämtadMedlem;

        [ObservableProperty]
        private Träningspass valdTräningspass;

        [ObservableProperty]
        private ObservableCollection<Träningspass> passLista;

        public MedlemMenyViewModel()
        {
            _säkerhetsController = new SäkerhetsController();
            _träningspassController = new TräningspassController();
            KonverteraMedlem();
            LaddaTräningspass();
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
                Kön = medlem.Kön     
            };
        }

        private void LaddaTräningspass()
        {
            var passes = _träningspassController.HämtaAllaTräningspass();
            PassLista = new ObservableCollection<Träningspass>(passes);
        }

        [RelayCommand]
        private void VisaMerInformation(Träningspass träningspass)
        {
            if (träningspass != null)
            {
                var merInformationWindow = new MerInformationWindow(träningspass);
                merInformationWindow.Show();
                CloseAction?.Invoke();
            }
            else
            {
                MessageBox.Show("Ingen träningspass är valt.");
            }
        }

        [RelayCommand]
        private void SkapaTräningspass()
        {
            var skapaTräningspassWindow = new SkapaTräningspassWindow();
            skapaTräningspassWindow.Show();
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void MenyKnapp()
        {
            var hamburgerMenyWindow = new HamburgerMenyWindow();
            hamburgerMenyWindow.Show();
            CloseAction?.Invoke();
        }
    }
}
