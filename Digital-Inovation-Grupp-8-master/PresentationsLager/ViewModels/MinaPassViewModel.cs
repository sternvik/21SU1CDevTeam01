using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager.Entiteter;
using PresentationsLager.Models;
using PresentationsLager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class MinaPassViewModel : ObservableObject
    {
        private readonly SäkerhetsController _säkerhetsController;
        private readonly TräningspassController _träningspassController;
        private readonly MedlemTräningspassController _medlemTräningspassController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private int antalTräningspass;

        [ObservableProperty]
        private MedlemModel hämtadMedlem;

        [ObservableProperty]
        private Träningspass valdTräningspass;

        [ObservableProperty]
        private ObservableCollection<Träningspass> passLista;
        public MinaPassViewModel() 
        {
            _säkerhetsController = new SäkerhetsController();
            _träningspassController = new TräningspassController();
            _medlemTräningspassController = new MedlemTräningspassController();

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
            var passes = _medlemTräningspassController.HämtaPassFörMedlem(HämtadMedlem.MedlemID);
            PassLista = new ObservableCollection<Träningspass>(passes);
        }

        [RelayCommand]
        private void VisaMerInformation(Träningspass träningspass)
        {
            if (träningspass != null)
            {
                var avbokaPassWindow = new AvbokaPassWindow(träningspass);
                avbokaPassWindow.Show();
                CloseAction?.Invoke();
            }
            else
            {
                MessageBox.Show("Ingen träningspass är valt.");
            }
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
