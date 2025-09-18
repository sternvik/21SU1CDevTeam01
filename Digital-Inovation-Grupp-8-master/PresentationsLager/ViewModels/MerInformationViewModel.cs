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
    public partial class MerInformationViewModel : ObservableObject
    {
        private readonly SäkerhetsController _säkerhetsController;
        private readonly TräningspassController _träningspassController;
        private readonly MedlemTräningspassController _medlemTräningspassController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private ObservableCollection<TräningspassModel> träningspassLista = new();

        [ObservableProperty]
        private ObservableCollection<MedlemModel> deltagare = new();

        [ObservableProperty]
        private MedlemModel hämtadMedlem;

        [ObservableProperty]
        private Träningspass valdTräningspass;

        public MerInformationViewModel(Träningspass träningspass) 
        {
            _säkerhetsController = new SäkerhetsController();
            _träningspassController = new TräningspassController();
            _medlemTräningspassController = new MedlemTräningspassController();

            ValdTräningspass = träningspass;

            KonverteraMedlem();
            LaddaDeltagare();
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

        private int HämtaAntalDeltagare(int träningspassId)
        {
            return _medlemTräningspassController.HämtaDeltagareFörPass(träningspassId).Count();
        }

        private void LaddaDeltagare()
        {
            if (ValdTräningspass == null) return;
            var deltagareLista = _medlemTräningspassController.HämtaDeltagareFörPass(ValdTräningspass.TräningspassID)
                .Select(m => new MedlemModel
                {
                    MedlemID = m.MedlemID,
                    Namn = m.Namn,
                    Epost = m.Epost,
                    Telefonnummer = m.Telefonnummer
                })
                .ToList();

            Deltagare = new ObservableCollection<MedlemModel>(deltagareLista);
        }

        [RelayCommand]
        private void LäggTillDeltagare()
        {
            if (ValdTräningspass == null || HämtadMedlem == null) return;

            int nuvarandeDeltagare = HämtaAntalDeltagare(ValdTräningspass.TräningspassID);

            if (nuvarandeDeltagare >= ValdTräningspass.MaxDeltagare)
            {
                MessageBox.Show("Det går inte att lägga till fler deltagare. Maxgränsen har nåtts!");
                return;
            }

            _medlemTräningspassController.LäggTillMedlemPåPass(
                new Medlem { MedlemID = HämtadMedlem.MedlemID },
                new Träningspass { TräningspassID = ValdTräningspass.TräningspassID });

            ValdTräningspass.DeltagarAntal = HämtaAntalDeltagare(ValdTräningspass.TräningspassID);
            MessageBox.Show("Deltagare tillagd!");
            LaddaDeltagare();
            LaddaTräningspass();
        }

        private void LaddaTräningspass()
        {
            var bokadePassIds = _medlemTräningspassController
            .HämtaPassFörMedlem(HämtadMedlem.MedlemID)
            .Select(tp => tp.TräningspassID)
            .ToHashSet();

            var allaTräningspass = _träningspassController.HämtaAllaTräningspass()
                .Where(tp => tp.Datum > DateTime.Now.Date && !bokadePassIds.Contains(tp.TräningspassID))
                .Select(t => new TräningspassModel
                {
                    TräningspassID = t.TräningspassID,
                    Tempo = t.Tempo,
                    MaxDeltagare = t.MaxDeltagare,
                    Tid = t.Tid.ToString(@"hh\:mm"),
                    Plats = t.Plats,
                    Datum = t.Datum,
                    Distans = t.Distans,
                    Beskrivning = t.Beskrivning,
                    DeltagarAntal = HämtaAntalDeltagare(t.TräningspassID),
                })
                .ToList();

            TräningspassLista = new ObservableCollection<TräningspassModel>(allaTräningspass);
        }

        [RelayCommand]
        private void Tillbaka()
        {
            var medlemMenyWindow = new MedlemMenyWindow();
            medlemMenyWindow.Show();
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

